
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
        void Initialize(IDeploymentConfig deploymentConfig);
    }
}
