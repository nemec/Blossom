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
        /// <summary>
        /// Role defined for this task.
        /// </summary>
        public string Role { get; private set; }

        /// <summary>
        /// Defines the task as fulfilling part of a role.
        /// This task (and all dependencies) will be run
        /// for all hosts in the role.
        /// </summary>
        /// <param name="role">Role, as a string, to define for the task.</param>
        public RoleAttribute(string role)
        {
            Role = role;
        }
    }
}
