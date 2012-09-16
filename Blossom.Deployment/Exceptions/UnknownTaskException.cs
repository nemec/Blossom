using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blossom.Deployment.Exceptions
{
    public class UnknownTaskException : TaskDependencyException
    {
        public UnknownTaskException(string message)
            : base(message) { }
    }
}
