using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blossom.Deployment.Exceptions
{
    public class TaskDependencyException : Exception
    {
        public TaskDependencyException(string message)
            : base(message) { }
    }
}
