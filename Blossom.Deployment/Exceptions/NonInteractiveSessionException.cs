using System;

namespace Blossom.Deployment.Exceptions
{
    /// <summary>
    /// This session disallows asking the user for input.
    /// </summary>
    public class NonInteractiveSessionException : Exception
    {
        internal NonInteractiveSessionException() { }

        internal NonInteractiveSessionException(string message)
            : base(message) { }

        internal NonInteractiveSessionException(string message, Exception innerException)
            : base(message, innerException) { }
    }
}