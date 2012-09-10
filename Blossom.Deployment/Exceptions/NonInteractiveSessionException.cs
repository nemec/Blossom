using System;

namespace Blossom.Deployment
{
    internal class NonInteractiveSessionException : Exception
    {
        public NonInteractiveSessionException()
            : base() { }

        public NonInteractiveSessionException(string message)
            : base(message) { }

        public NonInteractiveSessionException(string message, Exception innerException)
            : base(message, innerException) { }
    }
}