using Blossom.Deployment;
using System.Collections.Generic;
using Blossom.Deployment.Attributes;

namespace Blossom.Scripting
{
    public class CustomDeploymentConfig : DeploymentConfig
    {
        private List<Host> _hosts = new List<Host>
            {
                new Host
                    {
                        Hostname = "192.168.1.1",
                        Username = "username",
                        Password = "password"
                    }
            };

        public override List<Host> Hosts
        {
            get { return _hosts; }
            set { _hosts = value; }
        }


        public override bool DryRun { get; set; }
    }

    [Deployment]
    public class Test
    {
        [Task]
        public void Run(IDeploymentContext context)
        {
            context.Logger.Info(context.RemoteOps.RunCommand("ls"));
        }
    }
}