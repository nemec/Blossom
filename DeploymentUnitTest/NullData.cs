using Blossom;
using Blossom.Manager;

namespace DeploymentUnitTest
{
    /// <summary>
    /// <see cref="DeploymentManager{TDeploymentTasks,TTaskConfig}"/> with no TTaskConfig.
    /// </summary>
    /// <typeparam name="TDeploymentTasks"></typeparam>
    public class DeploymentManager<TDeploymentTasks>
        : DeploymentManager<TDeploymentTasks, NullConfig> where TDeploymentTasks : IDeploymentTasks<NullConfig>, new()
    {
        /// <summary>
        /// Create a new deploymentManager.
        /// </summary>
        /// <param name="config"></param>
        public DeploymentManager(DeploymentConfig<NullConfig> config)
            : base(config) { }
    }

    public interface IDeployment : IDeploymentTasks<NullConfig> { }

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
