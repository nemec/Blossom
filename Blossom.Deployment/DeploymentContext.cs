using Blossom.Deployment.Dependencies;
using Blossom.Deployment.Environments;
using Blossom.Deployment.Logging;
using Blossom.Deployment.Operations;
using Blossom.Deployment.Ssh;
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
        public ILogger Logger { get; set; }

        public Env Environment { get; set; }

        public ILocalOperations LocalOps { get; private set; }

        public IRemoteOperations RemoteOps { get; private set; }

        private Dictionary<string, string> _sessionConfig { get; set; }

        public DeploymentContext(IDeploymentConfig config)
        {
            this.Environment = new Env();
            Initialize(config);
        }

        public DeploymentContext(IDeploymentConfig config, IEnvironment remoteEnvironment)
        {
            this.Environment = new Env(remoteEnvironment);
            Initialize(config);
        }

        private void Initialize(IDeploymentConfig config)
        {
            Logger = new SimpleConsoleLogger();
            this.Environment.Hosts = config.Hosts;
        }

        private static IEnumerable<MethodInfo> SortTasksInObjectByPriorityAscending(object obj)
        {
            return obj.GetType().GetMethods()
                .Select(m => new
                {
                    Attr = (TaskAttribute)m.GetCustomAttributes(
                        typeof(TaskAttribute), true).FirstOrDefault(),
                    Method = m
                })
                .Where(k => k.Attr != null)
                .Select(k => k.Method);
        }

        public void BeginDeployment(object deploymentInstance)
        {
            BeginDeployment(new object[] { deploymentInstance });
        }

        public void BeginDeployment(IEnumerable<object> taskBlocks)
        {
            foreach (var host in this.Environment.Hosts)
            {
                Logger.Info(String.Format("Beginning deployment for {0}.", host));
                this.Environment.CurrentHost = host;

                try
                {
                    using (var ops = new BasicOperations(this,
                        new ShellWrapper(host), new SftpWrapper(host)))
                    {
                        LocalOps = ops;
                        RemoteOps = ops;

                        foreach (var taskBlock in taskBlocks)
                        {
                            var dependencyResolver = new DependencyResolver(taskBlock);
                            foreach (var method in dependencyResolver.OrderTasks())
                            {
                                Logger.Info("Beginning task: " + method.Name);
                                var parameters = method.GetParameters();

                                if (parameters.Length == 0)
                                {
                                    try
                                    {
                                        method.Invoke(taskBlock, null);
                                    }
                                    catch (TargetInvocationException exception)
                                    {
                                        throw exception.InnerException;
                                    }
                                }
                                else if (parameters.Length == 1 &&
                                    parameters.First().ParameterType == typeof(IDeploymentContext))
                                {
                                    try
                                    {
                                        method.Invoke(taskBlock, new[] { this });
                                    }
                                    catch (TargetInvocationException exception)
                                    {
                                        throw exception.InnerException;
                                    }
                                }
                                else
                                {
                                    throw new ArgumentException(
                                        "Task method must either take no parameters or a DeploymentContext as its sole parameter.");
                                }
                            }
                        }
                    }
                }
                catch (SshException exception)
                {
                    Logger.Fatal(exception.Message, exception);
                }

                #region Clean up getters

                LocalOps = null;
                RemoteOps = null;
                this.Environment.CurrentHost = null;

                #endregion
            }
        }
    }
}