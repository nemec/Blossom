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
        private TTaskConfig TaskConfig { get; set; }

        private bool DryRun { get; set; }

        public ILogger Logger { get; private set; }

        public ILocalOperations LocalOps { get; private set; }

        public IRemoteOperations RemoteOps { get; private set; }

        public IEnvironment RemoteEnv { get; set; }

        public IEnvironment LocalEnv { get; set; }

        public IHost Host { get; private set; }

        public InteractionType InteractionType { get; set; }

        /// <summary>
        /// Build a context for this deployment.
        /// </summary>
        /// <param name="host">Host attached to this context.</param>
        /// <param name="config">Configuration settings for this deployment.</param>
        public DeploymentContext(IHost host, DeploymentConfig<TTaskConfig> config)
        {
            Initialize(host, config);
        }

        /// <summary>
        /// Set up a DeploymentContext for a host with a specific remote environment.
        /// </summary>
        /// <param name="host"></param>
        /// <param name="config"></param>
        /// <param name="remoteEnvironment"></param>
        public DeploymentContext(IHost host, DeploymentConfig<TTaskConfig> config, IEnvironment remoteEnvironment)
        {
            RemoteEnv = remoteEnvironment;
            Initialize(host, config);
        }

        private void Initialize(IHost host, DeploymentConfig<TTaskConfig> config)
        {
            Logger = config.Logger;
            Host = host;
            DryRun = config.DryRun;
            TaskConfig = config.TaskConfig;
            InteractionType = InteractionType.AskForInput;
            LocalEnv = EnvironmentFinder.AutoDetermineLocalEnvironment(
                AppDomain.CurrentDomain.BaseDirectory);
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
                try
                {
                    // TODO some sort of Factory pattern
                    if (DryRun)
                    {
                        LocalOps = new DryRunLocalOperations(Logger);
                    }
                    else
                    {
                        LocalOps = new BasicLocalOperations(this);
                    }

                    if (DryRun)
                    {
                        RemoteOps = new DryRunRemoteOperations(Logger);
                    }
                    // If host is loopback, short circuit the network
                    else if (Host.Hostname == Blossom.Host.LoopbackHostname)
                    {
                        RemoteOps = new LoopbackRemoteOperations(this);
                    }
                    else
                    {
                        RemoteOps = new BasicRemoteOperations(this, Host);
                    }

                    var origin = new TDeploymentTasks
                        {
                            Context = this, 
                            Config = TaskConfig
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
                    // TODO retries
                    Logger.Error(exception.Message);
                }
                catch (SshException exception)
                {
                    Logger.Fatal(exception.Message, exception);
                }
                finally
                {
                    if (RemoteOps != null)
                    {
                        RemoteOps.Dispose();
                        RemoteOps = null;
                    }
                }
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
            }
        }

        private string HostToString(IHost host)
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
    }
}