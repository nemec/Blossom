using System;

namespace Blossom.Deployment.Attributes
{
    /// <summary>
    /// Defines the task as fulfilling part of a role.
    /// This task (and all dependencies) will be run
    /// for all hosts in the role.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public sealed class RoleAttribute : Attribute
    {
        public string Role { get; private set; }

        public RoleAttribute(string role)
        {
            Role = role;
        }
    }
}
