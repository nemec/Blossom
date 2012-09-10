using Blossom.Deployment.Logging;
using System.Collections.Generic;

namespace Blossom.Deployment
{
    public interface IDeploymentContext
    {
        ILogger Logger { get; set; }

        Env Environment { get; set; }

        Operations Operations { get; }

        void BeginDeployment(string[] args, object taskInstance,
            Dictionary<string, string> sessionConfig = null);
    }
}