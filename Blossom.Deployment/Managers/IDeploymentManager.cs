using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blossom.Deployment
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
