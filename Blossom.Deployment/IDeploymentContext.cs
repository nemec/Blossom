using System;
using System.Reflection;
using Blossom.Deployment.Logging;
using Blossom.Deployment.Operations;
using System.Collections.Generic;

namespace Blossom.Deployment
{
    public interface IDeploymentContext
    {
        /// <summary>
        /// A logging object used to display messages to the user.
        /// </summary>
        ILogger Logger { get; set; }

        Env Environment { get; set; }

        ILocalOperations LocalOps { get; }

        IRemoteOperations RemoteOps { get; }

        void BeginDeployment(IEnumerable<MethodInfo> tasks);
    }
}