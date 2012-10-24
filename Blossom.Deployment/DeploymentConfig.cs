using System.Collections.Generic;

namespace Blossom.Deployment
{
    public class DeploymentConfig
    {
        public virtual List<Host> Hosts { get; set; }
        public virtual bool DryRun { get; set; }
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
