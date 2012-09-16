using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blossom.Deployment
{
    /// <summary>
    /// Indicates to the dependency resolver that this task
    /// may be run multiple times. If enabled, the task
    /// will be run once every time it occurs as a
    /// *direct dependency* of another task.
    /// 
    /// If <paramref name="Standalone"/> is true (the default),
    /// the task will be run once on its own as well as every
    /// time it's a dependency. Otherwise, the taskwill only be
    /// run if it is a dependency of another task.
    /// 
    /// A direct dependency is defined as a task that is
    /// referenced by name in a <see cref="DependsAttribute"/>
    /// in another task.
    /// </summary>
    public class AllowMultipleExecutionAttribute : Attribute
    {
        public bool Standalone { get; set; }

        public AllowMultipleExecutionAttribute()
        {
            Standalone = true;
        }
    }
}
