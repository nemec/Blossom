using System;

namespace Blossom.Deployment.Exceptions
{
    public class AbortExecutionException : Exception
    {
        public AbortExecutionException()
        {
        }

        public AbortExecutionException(string reason)
            : base(reason)
        {
        }

        public AbortExecutionException(string reason, Exception innerException)
            : base(reason, innerException)
        {
        }
    }
}
