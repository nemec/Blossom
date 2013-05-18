using System;

namespace Blossom.Deployment.Exceptions
{
    /// <summary>
    /// Execution of this deployment instance was aborted.
    /// </summary>
    public class AbortExecutionException : Exception
    {
        internal AbortExecutionException()
        {
        }

        internal AbortExecutionException(string reason)
            : base(reason)
        {
        }

        internal AbortExecutionException(string reason, Exception innerException)
            : base(reason, innerException)
        {
        }
    }
}
