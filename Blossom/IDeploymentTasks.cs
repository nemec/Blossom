namespace Blossom
{
    /// <summary>
    /// Class that contains the set of tasks executed in a deployment.
    /// </summary>
    /// <typeparam name="TTaskConfig">Custom task configuration type.</typeparam>
    public interface IDeploymentTasks<TTaskConfig> : IDeploymentTasks
    {
        /// <summary>
        /// Custom configuration settings for this deployment instance.
        /// </summary>
        TTaskConfig Config { get; set; }
    }

    /// <summary>
    /// Class that contains the set of tasks executed in a deployment.
    /// </summary>
    public interface IDeploymentTasks
    {
        /// <summary>
        /// The context for this deployment instance.
        /// </summary>
        IDeploymentContext Context { get; set; }
    }
}
