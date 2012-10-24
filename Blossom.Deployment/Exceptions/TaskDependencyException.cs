using System;

namespace Blossom.Deployment.Exceptions
{
    public class TaskDependencyException : Exception
    {
        public TaskDependencyException(string message)
            : base(message) { }
    }
}
