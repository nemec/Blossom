using Blossom;
using Blossom.Manager;

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

    public class NullConfig : IConfig
    {
        public void InitializeDeployment(IDeploymentConfig deploymentConfig)
        {
        }

        public void InitializeContext(IDeploymentContext context)
        {
        }
    }

    public class DeploymentConfig : DeploymentConfig<NullConfig> { }
}
