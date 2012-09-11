using System;

namespace Blossom.Deployment
{
    public class NonInteractiveSessionException : Exception
    {
        public NonInteractiveSessionException()
            : base() { }

        public NonInteractiveSessionException(string message)
            : base(message) { }

        public NonInteractiveSessionException(string message, Exception innerException)
            : base(message, innerException) { }
    }
}