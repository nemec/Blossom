namespace Blossom.Deployment
{
    /// <summary>
    /// Class that contains the set of tasks executed in a deployment.
    /// </summary>
    /// <typeparam name="TTaskConfig">Custom task configuration type.</typeparam>
    public interface IDeployment<in TTaskConfig>
    {
        /// <summary>
        /// Initialize the current deployment with the given
        /// context and custom configuration settings.
        /// </summary>
        /// <param name="context">Context for this deployment instance.</param>
        /// <param name="config">Configuration settings for this deployment instance.</param>
        void InitializeDeployment(IDeploymentContext context, TTaskConfig config);
    }
}
