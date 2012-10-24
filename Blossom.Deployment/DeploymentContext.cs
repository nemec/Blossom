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
    public class DeploymentContext : IDeploymentContext
    {
        private bool DryRun { get; set; }

        public ILogger Logger { get; set; }

        public Env Environment { get; set; }

        public ILocalOperations LocalOps { get; private set; }

        public IRemoteOperations RemoteOps { get; private set; }

        public DeploymentContext(DeploymentConfig config)
        {
            Environment = new Env();
            Initialize(config);
        }

        public DeploymentContext(DeploymentConfig config, IEnvironment remoteEnvironment)
        {
            Environment = new Env(remoteEnvironment);
            Initialize(config);
        }

        private void Initialize(DeploymentConfig config)
        {
            Logger = new SimpleConsoleLogger();
            Environment.Hosts = config.Hosts;
            DryRun = config.DryRun;
        }

        public void BeginDeployment(IEnumerable<Invokable> tasks)
        {
            Logger.Context = this;

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
                            foreach (var task in tasks)
                            {
                                Logger.Info("Beginning task: " + task.Method.Name);
                                try
                                {
                                    task.Invoke(this);
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