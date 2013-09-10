namespace Blossom.Exceptions
{
    /// <summary>
    /// The task dependency chain forms a closed loop.
    /// </summary>
    public class CircularTaskDependencyException : TaskDependencyException
    {
        internal CircularTaskDependencyException(string message)
            : base(message) { }
    }
}
