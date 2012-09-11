using Blossom.Deployment.Environments;
using Blossom.Deployment.Logging;
using Blossom.Deployment.Ssh;
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

        public IOperations Operations { get; private set; }

        public DeploymentContext()
        {
            Logger = new ConsoleLogger();
            this.Environment = new Env();
        }

        public DeploymentContext(IEnvironment remoteEnvironment)
        {
            Logger = new ConsoleLogger();
            this.Environment = new Env(remoteEnvironment);
        }

        private static IEnumerable<MethodInfo> SortTasksInObjectByPriority(object obj)
        {
            return obj.GetType().GetMethods()
                .Select(m => new
                {
                    Attr = (TaskAttribute)m.GetCustomAttributes(
                        typeof(TaskAttribute), true).FirstOrDefault(),
                    Method = m
                })
                .Where(k => k.Attr != null)
                .OrderBy(k => k.Attr.Priority)
                .Select(k => k.Method);
        }

        public void BeginDeployment(string[] args, object taskInstance,
            Dictionary<string, string> sessionConfig = null)
        {
            // http://www.tamirgal.com/blog/page/SharpSSH.aspx
            

            foreach (var host in this.Environment.Hosts)
            {
                Logger.Info(String.Format("Beginning deployment for {0}.", host));
                //var session = new SessionWrapper(host);


                /*session.UserInfo = new ConsoleSessionUserInfo(this)
                {
                    Password = host.Password,
                    AutoRespondYN = AutoResponse.Yes
                };*/
                //session.SetConfig(sessionConfig);

                this.Environment.CurrentHost = host;
                //session.Connect();

                using (Operations = new Operations(this, new SftpWrapper(host)))
                {

                    foreach (var method in SortTasksInObjectByPriority(taskInstance))
                    {
                        var parameters = method.GetParameters();

                        if (parameters.Length == 0)
                        {
                            method.Invoke(taskInstance, null);
                        }
                        else if (parameters.Length == 1 &&
                            parameters.First().ParameterType == typeof(IDeploymentContext))
                        {
                            method.Invoke(taskInstance, new[] { this });
                        }
                        else
                        {
                            throw new ArgumentException(
                                "Task method must either take no parameters or a DeploymentContext as its sole parameter.");
                        }
                    }
                }
                Operations = null;

                this.Environment.CurrentHost = null;
            }
        }
    }
}