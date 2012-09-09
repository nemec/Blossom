using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blossom.Deployment
{
    class NonInteractiveSessionException : Exception
    {
        public NonInteractiveSessionException()
            : base() {}

        public NonInteractiveSessionException(string message)
            : base(message) {}

        public NonInteractiveSessionException(string message, Exception innerException)
            : base(message, innerException) { }
    }
}
