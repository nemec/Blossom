using System;

namespace Blossom.Deployment.Attributes
{
    /// <summary>
    /// Indicates to the deployment code which methods define tasks
    /// to be performed for the deployment. Each task may be optionally
    /// annotated with a priority to manage the order in which
    /// tasks are executed. Lower priority tasks are run first and
    /// all tasks without a priority are performed last.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public sealed class TaskAttribute : Attribute
    {
        /// <summary>
        /// Short description of the task. Suitable
        /// for displaying in a help menu.
        /// </summary>
        public string Description { get; set; }
    }
}