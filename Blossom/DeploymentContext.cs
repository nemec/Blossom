using System.Dynamic;
using System.Linq;
using System.Net.Sockets;
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
    /// <typeparam name="TDeployment">
    ///     Type of the class holding all tasks for this deployment.
    /// </typeparam>
    /// <typeparam name="TTaskConfig">
    ///     Type of the custom configuration object provided
    ///     to each <see cref="IDeploymentContext"/>.
    /// </typeparam>
    internal class DeploymentContext<TDeployment, TTaskConfig>
        : IDeploymentContext where TDeployment : IDeployment<TTaskConfig>, new()
    {
        private TTaskConfig TaskConfig { get; set; }

        private bool DryRun { get; set; }

        public ILogger Logger { get; private set; }

        public Env Environment { get; private set; }

        public dynamic Extras { get; set; } 

        public ILocalOperations LocalOps { get; private set; }

        public IRemoteOperations RemoteOps { get; private set; }

        /// <summary>
        /// Build a context for this deployment.
        /// </summary>
        /// <param name="host">Host attached to this context.</param>
        /// <param name="config">Configuration settings for this deployment.</param>
        public DeploymentContext(Host host, DeploymentConfig<TTaskConfig> config)
        {
            Environment = new Env();
            Initialize(host, config);
        }

        /// <summary>
        /// Set up a DeploymentContext for a host with a specific remote environment.
        /// </summary>
        /// <param name="host"></param>
        /// <param name="config"></param>
        /// <param name="remoteEnvironment"></param>
        public DeploymentContext(Host host, DeploymentConfig<TTaskConfig> config, IEnvironment remoteEnvironment)
        {
            Environment = new Env(remoteEnvironment);
            Initialize(host, config);
        }

        private void Initialize(Host host, DeploymentConfig<TTaskConfig> config)
        {
            Logger = config.Logger;
            Environment.Host = host;
            DryRun = config.DryRun;
            TaskConfig = config.TaskConfig;
            Extras = new ExpandoObject();
        }

        public void BeginDeployment(IEnumerable<MethodInfo> tasks)
        {
            if(!tasks.Any())
            {
                Logger.Warn("No tasks found for deployment.");
                return;
            }
            try
            {
                var host = Environment.Host;
                Logger.Info(String.Format("Beginning deployment for {0}.", host));
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
                    else if (host.Hostname == Host.LoopbackHostname)
                    {
                        RemoteOps = new LoopbackRemoteOperations(this);
                    }
                    else
                    {
                        if (host.Password == null)
                        {
                            host.Password = String.Empty;
                        }
                        RemoteOps = new BasicRemoteOperations(this, host);
                    }

                    var origin = new TDeployment
                        {
                            Context = this, 
                            Config = TaskConfig
                        };

                    foreach (var task in tasks)
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
    }
}