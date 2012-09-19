using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Blossom.Deployment
{
    public class ExecutionPlan : IEquatable<ExecutionPlan>
    {
        public Host Host { get; private set; }

        public IEnumerable<Invokable> TaskOrder { get; private set; }

        public ExecutionPlan(Host host, IEnumerable<Invokable> taskOrder)
        {
            Host = host;
            TaskOrder = taskOrder;
        }

        public bool Equals(ExecutionPlan other)
        {
            return other != null &&
                this.Host == other.Host &&
                this.TaskOrder.SequenceEqual(other.TaskOrder);
        }

        public override bool Equals(object obj)
        {
            if (this == null && obj == null)
            {
                return true;
            }
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
