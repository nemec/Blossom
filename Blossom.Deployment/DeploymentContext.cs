using CommandLine;
using Blossom.Deployment.ContextManagers;
using Blossom.Deployment.Environments;
using Blossom.Deployment.Logging;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Tamir.SharpSsh;
using Tamir.SharpSsh.jsch;

namespace Blossom.Deployment
{
    public class DeploymentContext
    {
        public ILogger Logger { get; set; }
        public Env Environment { get; set; }
        public Operations Operations { get; private set; }

        public DeploymentContext()
        {
            Logger = new ConsoleLogger();
            this.Environment = new Env();
        }

        public DeploymentContext(IEnvironment remoteEnvironment)
        {
            Logger = new ConsoleLogger();
            this.Environment = new Env();
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

        private Session SetupHostSession(JSch jsch, Host host, Dictionary<string, string> sessionConfig)
        {
            var session = jsch.getSession(
                    host.Username ?? System.Environment.UserName,
                    host.Hostname,
                    host.Port != 0 ? host.Port : 22);

            session.setUserInfo(new ConsoleSessionUserInfo(host.Password,
                autoRespondYN: AutoResponse.Yes));
            session.setConfig(new Hashtable(sessionConfig));
            session.connect();

            this.Environment.CurrentHost = host;
            this.Environment.CurrentSession = session;
            return session;
        }

        public void BeginDeployment(string[] args, object tasks,
            Dictionary<string, string> sessionConfig = null)
        {
            // http://www.tamirgal.com/blog/page/SharpSSH.aspx
            var jsch = new JSch();

            foreach (var host in this.Environment.Hosts)
            {
                Logger.Info(String.Format("Beginning deployment for {0}.", host));
                var session = SetupHostSession(jsch, host, sessionConfig);

                Operations = new Operations(this);
                
                foreach (var method in SortTasksInObjectByPriority(tasks))
                {
                    var parameters = method.GetParameters();

                    if (parameters.Length == 0)
                    {
                        method.Invoke(tasks, null);
                    }
                    else if (parameters.Length == 1 &&
                        parameters.First().ParameterType == typeof(DeploymentContext))
                    {
                        method.Invoke(tasks, new[] { this });
                    }
                    else
                    {
                        throw new ArgumentException(
                            "Task method must either take no parameters or a DeploymentContext as its sole parameter.");
                    }
                }

                this.Environment.CurrentHost = null;
                this.Environment.CurrentSession = null;
                session.disconnect();
            }
        }
    }
}
