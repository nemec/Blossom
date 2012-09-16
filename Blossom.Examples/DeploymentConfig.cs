using Blossom.Deployment;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blossom.Examples
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
