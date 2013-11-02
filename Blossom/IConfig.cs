
namespace Blossom
{
    /// <summary>
    /// Shared config instance between all deployment hosts.
    /// </summary>
    public interface IConfig
    {
        /// <summary>
        /// Initialize the deployment configuration, including which
        /// hosts to connect to.
        /// </summary>
        /// <param name="deploymentConfig"></param>
        void InitializeDeployment(IDeploymentConfig deploymentConfig);

        /// <summary>
        /// Initialize the deployment context, including the environment.
        /// </summary>
        /// <param name="context"></param>
        void InitializeContext(IDeploymentContext context);
    }
}
