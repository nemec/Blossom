using Blossom.Logging;

namespace Blossom
{
    /// <summary>
    /// Deployment infrastructure initialization interface
    /// </summary>
    public interface IDeploymentConfig
    {
        /// <summary>
        /// Deploy to these hosts.
        /// </summary>
        Host[] Hosts { get; set; }

        /// <summary>
        /// List of roles that need to be run.
        /// </summary>
        string[] Roles { get; set; }

        /// <summary>
        /// If true, run through the tasks but do not
        /// actually perform any operations. If one task
        /// depends on the output of another, tasks may not
        /// execute correctly.
        /// </summary>
        bool DryRun { get; set; }

        /// <summary>
        /// Logging mechanism.
        /// </summary>
        ILogger Logger { get; set; }
    }

    /// <summary>
    /// Deployment infrastructure configuration settings.
    /// </summary>
    /// <typeparam name="TTaskConfig">Custom configuration type for the deployment.</typeparam>
    public class DeploymentConfig<TTaskConfig> : IDeploymentConfig
    {
        /// <summary>
        /// Deployment infrastructure configuration settings.
        /// </summary>
        public DeploymentConfig()
        {
            Hosts = new Host[0];
            Roles = new string[0];
            Logger = new SimpleConsoleLogger();
        } 

        /// <summary>
        /// Deploy to these hosts.
        /// </summary>
        public Host[] Hosts { get; set; }

        /// <summary>
        /// List of roles that need to be run.
        /// </summary>
        public string[] Roles { get; set; }

        /// <summary>
        /// If true, run through the tasks but do not
        /// actually perform any operations. If one task
        /// depends on the output of another, tasks may not
        /// execute correctly.
        /// </summary>
        public bool DryRun { get; set; }

        /// <summary>
        /// Logging mechanism.
        /// </summary>
        public ILogger Logger { get; set; }

        /// <summary>
        /// Custom deployment-specific configuration settings.
        /// </summary>
        public TTaskConfig TaskConfig { get; set; }
    }
}
