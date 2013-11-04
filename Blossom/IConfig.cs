
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

    /// <summary>
    /// A basic <see cref="IConfig"/> implementation that does no
    /// initialization.
    /// </summary>
    public class BaseConfig : IConfig
    {
        /// <inheritdoc />
        public virtual void InitializeDeployment(IDeploymentConfig deploymentConfig)
        {
        }

        /// <inheritdoc />
        public virtual void InitializeContext(IDeploymentContext context)
        {
        }
    }
}
