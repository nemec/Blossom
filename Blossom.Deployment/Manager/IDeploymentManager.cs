using System;
using System.Collections.Generic;

namespace Blossom.Deployment.Manager
{
    /// <summary>
    /// Describes an object that creates
    /// <see cref="ExecutionPlan"/>s and runs
    /// multiple deployments on multiple hosts.
    /// </summary>
    public interface IDeploymentManager
    {
        IEnumerable<Tuple<string, string>> GetAvailableCommands();

        IEnumerable<ExecutionPlan> GetExecutionPlans();

        void BeginDeployments();
    }
}
