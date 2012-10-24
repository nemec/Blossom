using System;

namespace Blossom.Deployment.Attributes
{
    /// <summary>
    /// Indicates to the deployment code which classes
    /// contain tasks.
    /// 
    /// Usually ignored, but may be useful in cases where
    /// an assembly must be searched dynamically for
    /// classes containing tasks.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class DeploymentAttribute : Attribute
    {
    }
}
