using System;
using System.Reflection;
using Blossom.Deployment.Logging;
using Blossom.Deployment.Operations;
using System.Collections.Generic;

namespace Blossom.Deployment
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
        ILogger Logger { get; set; }

        /// <summary>
        /// Access to both local and remote environment.
        /// </summary>
        Env Environment { get; set; }

        /// <summary>
        /// Set of available operations to perform on the local machine.
        /// </summary>
        ILocalOperations LocalOps { get; }

        /// <summary>
        /// Set of available operations to perform on the remote machine.
        /// </summary>
        IRemoteOperations RemoteOps { get; }

        /// <summary>
        /// Use the current DeploymentContext to execute a set of tasks.
        /// </summary>
        /// <param name="tasks">Set of tasks to run, in order.</param>
        void BeginDeployment(IEnumerable<MethodInfo> tasks);
    }
}