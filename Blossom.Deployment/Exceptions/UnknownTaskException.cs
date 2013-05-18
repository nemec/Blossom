namespace Blossom.Deployment.Exceptions
{
    /// <summary>
    /// The task specified as a dependency cannot be found.
    /// </summary>
    public class UnknownTaskException : TaskDependencyException
    {
        internal UnknownTaskException(string message)
            : base(message) { }
    }
}
