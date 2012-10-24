namespace Blossom.Deployment.Exceptions
{
    public class CircularTaskDependencyException : TaskDependencyException
    {
        public CircularTaskDependencyException(string message)
            : base(message) { }
    }
}
