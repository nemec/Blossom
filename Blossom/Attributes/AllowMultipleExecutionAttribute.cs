using System;

namespace Blossom.Attributes
{
    /// <summary>
    /// Indicates to the dependency resolver that this task
    /// may be run multiple times. If enabled, the task
    /// will be run once every time it occurs as a
    /// *direct dependency* of another task.
    /// 
    /// If <see cref="Standalone"/> is true (the default),
    /// the task will be run once on its own as well as every
    /// time it's a dependency. Otherwise, the task will only be
    /// run if it is a dependency of another task.
    /// 
    /// A direct dependency is defined as a task that is
    /// referenced by name in a <see cref="DependsAttribute"/>
    /// in another task.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public sealed class AllowMultipleExecutionAttribute : Attribute
    {
        /// <summary>
        /// If <see cref="Standalone"/> is true (the default),
        /// the task will be run once on its own as well as every
        /// time it's a dependency. Otherwise, the taskwill only be
        /// run if it is a dependency of another task.
        /// </summary>
        public bool Standalone { get; set; }

        /// <summary>
        /// Indicate that this task may be executed multiple
        /// times. By default, <see cref="Standalone"/> is
        /// set to true.
        /// </summary>
        public AllowMultipleExecutionAttribute()
        {
            Standalone = true;
        }
    }
}
