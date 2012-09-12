using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blossom.Deployment
{
    public interface IDeploymentConfig
    {
        List<Host> Hosts { get; set; }
    }

    public static class IDeploymentConfigExtensions
    {
        public static void MergeFrom(this IDeploymentConfig target, IDeploymentConfig config)
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
