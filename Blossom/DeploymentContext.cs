using System.Linq;
using System.Net.Sockets;
using System.Text;
using Blossom.Environments;
using Blossom.Exceptions;
using Blossom.Logging;
using Blossom.Operations;
using Renci.SshNet.Common;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Blossom
{
    /// <summary>
    /// Manages the environment for a single deployment on
    /// a single host. Each context is independent from any
    /// other deployment context.
    /// </summary>
    /// <typeparam name="TDeploymentTasks">
    ///     Type of the class holding all tasks for this deployment.
    /// </typeparam>
    /// <typeparam name="TTaskConfig">
    ///     Type of the custom configuration object provided
    ///     to each <see cref="IDeploymentContext"/>.
    /// </typeparam>
    internal class DeploymentContext<TDeploymentTasks, TTaskConfig>
        : IDeploymentContext where TDeploymentTasks : IDeploymentTasks<TTaskConfig>, new()
    {
        private DeploymentConfig<TTaskConfig> DeploymentConfig { get; set; }

        private ILocalOperationsFactory LocalOpsFactory { get; set; }

        private IRemoteOperationsFactory RemoteOpsFactory { get; set; }

        public ILogger Logger { get; private set; }

        public ILocalOperations LocalOps { get; private set; }

        public IRemoteOperations RemoteOps { get; private set; }

        public IEnvironment RemoteEnv { get; set; }

        public IEnvironment LocalEnv { get; set; }

        public IHost Host { get; private set; }

        public InteractionType InteractionType { get; set; }

        /// <summary>
        /// Set up a DeploymentContext for a host with a specific remote environment.
        /// </summary>
        /// <param name="host">Host attached to this context.</param>
        /// <param name="config">Configuration settings for this deployment.</param>
        /// <param name="remoteEnvironment"></param>
        /// <param name="localOperationsFactory"></param>
        /// <param name="remoteOperationsFactory"></param>
        public DeploymentContext(
            IHost host, 
            DeploymentConfig<TTaskConfig> config, 
            IEnvironment remoteEnvironment = null,
            ILocalOperationsFactory localOperationsFactory = null,
            IRemoteOperationsFactory remoteOperationsFactory = null)
        {
            RemoteEnv = remoteEnvironment;
            Logger = config.Logger;
            Host = host;
            DeploymentConfig = config;
            InteractionType = InteractionType.AskForInput;
            LocalEnv = EnvironmentFinder.AutoDetermineLocalEnvironment(
                AppDomain.CurrentDomain.BaseDirectory);

            LocalOpsFactory = localOperationsFactory ?? new LocalOperationsFactory
                {
                    Context = this,
                    DeploymentConfig = config
                };

            RemoteOpsFactory = remoteOperationsFactory ?? new RemoteOperationsFactory
                {
                    Context = this,
                    DeploymentConfig = config
                };
        }

        public void BeginDeployment(IEnumerable<MethodInfo> tasks)
        {
            var taskList = tasks.ToList();
            if(!taskList.Any())
            {
                Logger.Warn("No tasks found for deployment.");
                return;
            }
            try
            {
                Logger.Info(String.Format(
                    "Beginning deployment for {0}.", HostToString(Host)));
                var retriesRemaining = DeploymentConfig.MaxConnectionRetries;
                do
                {
                    try
                    {
                        LocalOps = LocalOpsFactory.GetOperations();
                        RemoteOps = RemoteOpsFactory.GetOperations();

                        var origin = new TDeploymentTasks
                            {
                                Context = this,
                                Config = DeploymentConfig.TaskConfig,
                            };

                        foreach (var task in taskList)
                        {
                            Logger.Info("Beginning task: " + task.Name);
                            try
                            {
                                task.Invoke(origin, null);
                            }
                            catch (TargetInvocationException exception)
                            {
                                throw exception.InnerException;
                            }
                        }
                    }
                    catch (SocketException exception)
                    {
                        Logger.Error(exception.Message);
                        DecrementRetriesAndAbortIfNecessary(retriesRemaining, exception);
                    }
                    catch (SshException exception)
                    {
                        Logger.Fatal(exception.Message, exception);
                        DecrementRetriesAndAbortIfNecessary(retriesRemaining, exception);
                    }
                    finally
                    {
                        if (RemoteOps != null)
                        {
                            RemoteOps.Dispose();
                            RemoteOps = null;
                        }
                    }
                } while (retriesRemaining > 0);
            }
            catch(AbortExecutionException exception)
            {
                try
                {
                    Logger.Abort(exception.Message, exception);
                }
                catch(AbortExecutionException)
                {
                    // We know it's going to just rethrow itself.
                    // Need to make sure an error message is logged
                    // stating that we're aborting.
                }
                throw;
            }
        }

        private static string HostToString(IHost host)
        {
            var builder = new StringBuilder();
            if (host.Username != null)
            {
                builder.Append(host.Username);
                builder.Append("@");
            }
            builder.Append(host.Hostname);
            if (host.Port != 0)
            {
                builder.Append(":");
                builder.Append(host.Port);
            }
            return builder.ToString();
        }

        private void DecrementRetriesAndAbortIfNecessary(
            int retriesRemaining, Exception context)
        {
            retriesRemaining--;
            if (retriesRemaining == 0)
            {
                if (!DeploymentConfig.SkipBadHosts)
                {
                    throw new AbortExecutionException(String.Format(
                        "Connection failed for host {0}.", HostToString(Host)),
                                                      context);
                }
            }
            else
            {
                Logger.Info(String.Format(
                    "Retrying {0} more time(s).", retriesRemaining));
            }
        }
    }
}