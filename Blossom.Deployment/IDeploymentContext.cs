using Blossom.Deployment.Logging;
using Blossom.Deployment.Operations;
using System.Collections.Generic;

namespace Blossom.Deployment
{
    public interface IDeploymentContext
    {
        ILogger Logger { get; set; }

        Env Environment { get; set; }

        ILocalOperations LocalOps { get; }

        IRemoteOperations RemoteOps { get; }

        void BeginDeployment(object deploymentInstance);

        void BeginDeployment(IEnumerable<object> deploymentInstances);
    }
}