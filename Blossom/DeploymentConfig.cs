using System.Collections.Generic;
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
        IEnumerable<IHost> Hosts { get; set; }

        /// <summary>
        /// List of roles that need to be run.
        /// </summary>
        IEnumerable<string> Roles { get; set; }

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

        /// <summary>
        /// Maximum number of connection attempts to each host.
        /// </summary>
        int MaxConnectionAttempts { get; set; }
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
            MaxConnectionAttempts = 1;
        }

        /// <inheritdoc />
        public IEnumerable<IHost> Hosts { get; set; }

        /// <inheritdoc />
        public IEnumerable<string> Roles { get; set; }

        /// <inheritdoc />
        public bool DryRun { get; set; }

        /// <inheritdoc />
        public ILogger Logger { get; set; }

        /// <summary>
        /// Custom deployment-specific configuration settings.
        /// </summary>
        public TTaskConfig TaskConfig { get; set; }

        /// <inheritdoc />
        public int MaxConnectionAttempts { get; set; }
    }
}
