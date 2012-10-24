namespace Blossom.Deployment.Exceptions
{
    public class UnknownTaskException : TaskDependencyException
    {
        public UnknownTaskException(string message)
            : base(message) { }
    }
}
