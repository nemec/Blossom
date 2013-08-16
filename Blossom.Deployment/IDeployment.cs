namespace Blossom.Deployment
{
    /// <summary>
    /// Class that contains the set of tasks executed in a deployment.
    /// </summary>
    /// <typeparam name="TTaskConfig">Custom task configuration type.</typeparam>
    public interface IDeployment<TTaskConfig> : IDeployment
    {
        /// <summary>
        /// Custom configuration settings for this deployment instance.
        /// </summary>
        TTaskConfig Config { get; set; }
    }

    /// <summary>
    /// Class that contains the set of tasks executed in a deployment.
    /// </summary>
    public interface IDeployment
    {
        /// <summary>
        /// The context for this deployment instance.
        /// </summary>
        IDeploymentContext Context { get; set; }
    }
}
