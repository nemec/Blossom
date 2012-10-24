using System;

namespace Blossom.Deployment.Exceptions
{
    public class NonInteractiveSessionException : Exception
    {
        public NonInteractiveSessionException() { }

        public NonInteractiveSessionException(string message)
            : base(message) { }

        public NonInteractiveSessionException(string message, Exception innerException)
            : base(message, innerException) { }
    }
}