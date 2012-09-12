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
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class DeploymentAttribute : Attribute
    {
    }
}
