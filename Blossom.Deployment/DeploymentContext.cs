using System.Linq;
using System.Net.Sockets;
using Blossom.Deployment.Environments;
using Blossom.Deployment.Exceptions;
using Blossom.Deployment.Logging;
using Blossom.Deployment.Operations;
using Renci.SshNet.Common;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Blossom.Deployment
{
    public class DeploymentContext<TDeployment, TConfig>
        : IDeploymentContext where TDeployment : IDeployment<TConfig>, new()
    {
        private TConfig TaskConfig { get; set; }

        private bool DryRun { get; set; }

        public ILogger Logger { get; set; }

        public Env Environment { get; set; }

        public ILocalOperations LocalOps { get; private set; }

        public IRemoteOperations RemoteOps { get; private set; }

        public DeploymentContext(DeploymentConfig<TConfig> config)
        {
            Environment = new Env();
            Initialize(config);
        }

        public DeploymentContext(DeploymentConfig<TConfig> config, IEnvironment remoteEnvironment)
        {
            Environment = new Env(remoteEnvironment);
            Initialize(config);
        }

        private void Initialize(DeploymentConfig<TConfig> config)
        {
            Logger = config.Logger;
            Environment.Hosts = config.Hosts;
            DryRun = config.DryRun;
            TaskConfig = config.TaskConfig;
        }

        public void BeginDeployment(IEnumerable<MethodInfo> tasks)
        {
            if(!tasks.Any())
            {
                Logger.Warn("No tasks found for deployment.");
                return;
            }

            foreach (var host in Environment.Hosts)
            {
                try
                {
                    Logger.Info(String.Format("Beginning deployment for {0}.", host));
                    Environment.CurrentHost = host;

                    try
                    {
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
                            RemoteOps = new BasicRemoteOperations(this, host);
                        }

                        using (RemoteOps)
                        {
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

                        #region Clean up getters

                        LocalOps = null;
                        RemoteOps = null;
                        Environment.CurrentHost = null;

                        #endregion
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
}