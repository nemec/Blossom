using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blossom.Deployment
{
    public class DeploymentConfig
    {
        public List<Host> Hosts { get; set; }
        public bool DryRun { get; set; }
    }

    public static class DeploymentConfigExtensions
    {
        public static void MergeFrom(this DeploymentConfig target, DeploymentConfig config)
        {
            #region Merge Hosts

            if (target.Hosts == null)
            {
                target.Hosts = config.Hosts;
            }
            else if(config.Hosts != null)
            {
                target.Hosts.AddRange(config.Hosts);
            }

            #endregion
        }
    }
}
