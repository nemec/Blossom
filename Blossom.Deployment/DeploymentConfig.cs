using System.Collections.Generic;
using Blossom.Deployment.Logging;

namespace Blossom.Deployment
{
    /// <summary>
    /// Deployment infrastructure configuration settings.
    /// </summary>
    /// <typeparam name="TTaskConfig">Custom configuration type for the deployment.</typeparam>
    public class DeploymentConfig<TTaskConfig>
    {
        /// <summary>
        /// Deploy to these hosts.
        /// </summary>
        public List<Host> Hosts { get; set; }

        /// <summary>
        /// List of roles that need to be run.
        /// </summary>
        public List<string> Roles { get; set; }
 
        /// <summary>
        /// If true, run through the tasks but do not
        /// actually perform any operations. If one task
        /// depends on the output of another, tasks may not
        /// execute correctly.
        /// </summary>
        public bool DryRun { get; set; }

        /// <summary>
        /// Logging mechanism.
        /// </summary>
        public ILogger Logger { get; set; }

        /// <summary>
        /// Custom deployment-specific configuration settings.
        /// </summary>
        public TTaskConfig TaskConfig { get; set; }
    }
}
