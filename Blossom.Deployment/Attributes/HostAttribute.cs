using System;

namespace Blossom.Deployment.Attributes
{
    /// <summary>
    /// Defines a host that the task should run on.
    /// The parameter may be either a hostname or
    /// an alias and the attribute may be used
    /// multiple times on a single Task.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public sealed class HostAttribute : Attribute
    {
        public string Host { get; private set; }

        public HostAttribute(string hostNameOrAlias)
        {
            Host = hostNameOrAlias;
        }
    }
}
