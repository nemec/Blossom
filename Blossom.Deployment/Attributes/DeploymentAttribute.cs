using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blossom.Deployment
{
    /// <summary>
    /// Indicates to the deployment code which classes
    /// contain tasks.
    /// 
    /// Usually ignored, but may be useful in cases where
    /// an assembly must be searched dynamically for
    /// classes containing tasks
    /// (see <seealso cref="Blossom.Scripting"/>).
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class DeploymentAttribute : Attribute
    {
    }
}
