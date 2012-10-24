using System;

namespace Blossom.Deployment.Attributes
{
    /// <summary>
    /// Allows a Task to indicate that it depends on another
    /// task. This dependent task will be guaranteed to
    /// execute before the current task.
    /// 
    /// If a Task is marked with multiple dependencies, order
    /// of execution of those dependencies is undefined
    /// (they may have dependencies of their own that could change
    /// execution order).
    /// 
    /// Any circular dependencies will throw an error at runtime.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public sealed class DependsAttribute : Attribute
    {
        /// <summary>
        /// The method name of the Task that the marked Task
        /// depends on.
        /// </summary>
        public string TaskName { get; private set; }

        /// <summary>
        /// Register a dependency to a Task of the given name.
        /// </summary>
        /// <param name="taskMethodName">Method name of the task.</param>
        public DependsAttribute(string taskMethodName)
        {
            TaskName = taskMethodName;
        }
    }
}
