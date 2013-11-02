using System;

namespace Blossom.Exceptions
{
    /// <summary>
    /// General task dependency error.
    /// </summary>
    public abstract class TaskDependencyException : Exception
    {
        /// <summary>
        /// A general task dependency error.
        /// </summary>
        /// <param name="message"></param>
        protected TaskDependencyException(string message)
            : base(message) { }
    }
}
