using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blossom.Deployment.Attributes
{
    /// <summary>
    /// Indicates that a <see cref="Task"/> is
    /// capable of being run in parallel with other
    /// tasks (aside from tasks that depend on it).
    /// 
    /// TODO Implement parallelization
    /// </summary>
    public sealed class ParallelizableAttribute : Attribute
    {
    }
}
