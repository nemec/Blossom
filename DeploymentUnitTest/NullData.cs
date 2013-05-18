using Blossom.Deployment;
using Blossom.Deployment.Manager;

namespace DeploymentUnitTest
{
    /// <summary>
    /// <see cref="DeploymentManager{TDeployment,TTaskConfig}"/> with no TTaskConfig.
    /// </summary>
    /// <typeparam name="TDeployment"></typeparam>
    public class DeploymentManager<TDeployment>
        : DeploymentManager<TDeployment, NullConfig> where TDeployment : IDeployment<NullConfig>, new()
    {
        /// <summary>
        /// Create a new deploymentManager.
        /// </summary>
        /// <param name="config"></param>
        public DeploymentManager(DeploymentConfig<NullConfig> config)
            : base(config) { }
    }

    public interface IDeployment : IDeployment<NullConfig> { }

    public class NullConfig { }

    public class DeploymentConfig : DeploymentConfig<NullConfig> { }
}
