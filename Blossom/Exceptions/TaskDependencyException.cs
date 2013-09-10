using System;

namespace Blossom.Exceptions
{
    /// <summary>
    /// General task dependency error.
    /// </summary>
    public abstract class TaskDependencyException : Exception
    {
        protected TaskDependencyException(string message)
            : base(message) { }
    }
}
