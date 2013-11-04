using System.Reflection;
using Blossom.Environments;
using Blossom.Logging;
using Blossom.Operations;
using System.Collections.Generic;

namespace Blossom
{
    /// <summary>
    /// Manages the environment for a single deployment on
    /// a single host. Each context is independent from any
    /// other deployment context.
    /// </summary>
    public interface IDeploymentContext
    {
        /// <summary>
        /// A logging object used to display messages to the user.
        /// </summary>
        ILogger Logger { get; }

        /// <summary>
        /// Current host information.
        /// </summary>
        IHost Host { get; }

        /// <summary>
        /// Allows for a guaranteed non-interactive session.
        /// If one of the deployment operations attempts to
        /// interact with the user (eg. through stdin),
        /// the deployment will abort with an error message.
        /// </summary>
        InteractionType InteractionType { get; set; }

        /// <summary>
        /// Set of available operations to perform on the local machine.
        /// </summary>
        ILocalOperations LocalOps { get; }

        /// <summary>
        /// Contains environment-specific data and operations for the
        /// local end of the connection.
        /// </summary>
        IEnvironment LocalEnv { get; }

        /// <summary>
        /// Set of available operations to perform on the remote machine.
        /// </summary>
        IRemoteOperations RemoteOps { get; }

        /// <summary>
        /// Contains environment-specific data and operations for the
        /// remote end of the connection.
        /// </summary>
        IEnvironment RemoteEnv { get; }

        /// <summary>
        /// Use the current DeploymentContext to execute a set of tasks.
        /// </summary>
        /// <param name="tasks">Set of tasks to run, in order.</param>
        void BeginDeployment(IEnumerable<MethodInfo> tasks);
    }
}