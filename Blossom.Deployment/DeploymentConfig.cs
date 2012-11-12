using System.Collections.Generic;
using Blossom.Deployment.Logging;

namespace Blossom.Deployment
{
    public class DeploymentConfig<TTaskConfig>
    {
        public List<Host> Hosts { get; set; }
        public bool DryRun { get; set; }
        public ILogger Logger { get; set; }
        public TTaskConfig TaskConfig { get; set; }
    }

    public class NullConfig { }

    public class DeploymentConfig : DeploymentConfig<NullConfig> { }
}
