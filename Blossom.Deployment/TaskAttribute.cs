using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blossom.Deployment
{
    /// <summary>
    /// Indicates to the deployment code which methods define tasks
    /// to be performed for the deployment. Each task may be optionally
    /// annotated with a priority to manage the order in which
    /// tasks are executed. Lower priority tasks are run first and
    /// all tasks without a priority are performed last.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class TaskAttribute : Attribute
    {
        /// <summary>
        /// Specifies the priority of a task.
        /// Tasks with lower priority will be executed first.
        /// </summary>
        public readonly int Priority;

        public TaskAttribute(int priority = Int32.MaxValue)
        {
            Priority = priority;
        }
    }
}
