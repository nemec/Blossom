using Blossom.Deployment;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blossom.Scripting
{
    public class DeploymentConfig : IDeploymentConfig
    {
        public List<Host> Hosts { get; set; }

        public DeploymentConfig(List<Host> hosts)
        {
            Hosts = hosts;
        }
    }
}
