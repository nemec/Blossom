using Blossom.Deployment.Logging;
using Blossom.Deployment.Operations;
using System.Collections.Generic;
using System.Reflection;

namespace Blossom.Deployment
{
    public interface IDeploymentContext
    {
        ILogger Logger { get; set; }

        Env Environment { get; set; }

        ILocalOperations LocalOps { get; }

        IRemoteOperations RemoteOps { get; }

        void BeginDeployment(IEnumerable<Invokable> tasks);
    }
}