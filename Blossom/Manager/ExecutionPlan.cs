using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Blossom.Manager
{
    /// <summary>
    /// A set of tasks to run on a host.
    /// </summary>
    public class ExecutionPlan : IEquatable<ExecutionPlan>
    {
        /// <summary>
        /// Host this plan is applied to.
        /// </summary>
        public Host Host { get; private set; }

        /// <summary>
        /// List of tasks to run, in order.
        /// </summary>
        public IEnumerable<MethodInfo> TaskOrder { get; private set; }

        /// <summary>
        /// Create a new execution plan.
        /// </summary>
        /// <param name="host">Plan's host.</param>
        /// <param name="taskOrder">Plan's tasks.</param>
        public ExecutionPlan(Host host, IEnumerable<MethodInfo> taskOrder)
        {
            Host = host;
            TaskOrder = taskOrder;
        }

        public bool Equals(ExecutionPlan other)
        {
            var matching = other != null &&
                           Equals(Host, other.Host);
            if(!matching)
            {
                return false;
            }
            foreach (var result in TaskOrder.Zip(other.TaskOrder, Tuple.Create))
            {
                if(!result.Item1.Equals(result.Item2))
                {
                    return false;
                }
            }
            return true;
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as ExecutionPlan);
        }

        public override int GetHashCode()
        {
            var num = 1962473570;
            num = num * Host.GetHashCode();
            foreach (var task in TaskOrder)
            {
                num = (num * task.GetHashCode()) % int.MaxValue;
            }
            return num;
        }
    }
}
