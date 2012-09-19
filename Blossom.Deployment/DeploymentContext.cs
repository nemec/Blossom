using Blossom.Deployment.Dependencies;
using Blossom.Deployment.Environments;
using Blossom.Deployment.Logging;
using Blossom.Deployment.Operations;
using Renci.SshNet.Common;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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

        private Dictionary<string, string> _sessionConfig { get; set; }

        public DeploymentContext(DeploymentConfig config)
        {
            this.Environment = new Env();
            Initialize(config);
        }

        public DeploymentContext(DeploymentConfig config, IEnvironment remoteEnvironment)
        {
            this.Environment = new Env(remoteEnvironment);
            Initialize(config);
        }

        private void Initialize(DeploymentConfig config)
        {
            Logger = new SimpleConsoleLogger();
            this.Environment.Hosts = config.Hosts;
            DryRun = config.DryRun;
        }

        public void BeginDeployment(IEnumerable<Invokable> tasks)
        {
            foreach (var host in this.Environment.Hosts)
            {
                Logger.Info(String.Format("Beginning deployment for {0}.", host));
                this.Environment.CurrentHost = host;

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
                    this.Environment.CurrentHost = null;

                    #endregion
                }
                catch (SshException exception)
                {
                    Logger.Fatal(exception.Message, exception);
                }
            }
        }
    }
}