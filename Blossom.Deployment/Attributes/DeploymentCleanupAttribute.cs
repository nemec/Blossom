using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blossom.Deployment
{
    /// <summary>
    /// Code to run after all tasks have been run.
    /// If a method is tagged as both a <see cref="Task"/>
    /// and <see cref="DeploymentCleanupAttribute"/>, the
    /// <see cref="Task"/> attribute is ignored for
    /// execution planning purposes.
    /// 
    /// Multiple methods tagged with this attribute are
    /// run in an undefined order.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class DeploymentCleanupAttribute : Attribute
    {
    }
}
