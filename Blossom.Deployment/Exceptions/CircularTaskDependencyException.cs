using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blossom.Deployment.Exceptions
{
    public class CircularTaskDependencyException : TaskDependencyException
    {
        public CircularTaskDependencyException(string message)
            : base(message) { }
    }
}
