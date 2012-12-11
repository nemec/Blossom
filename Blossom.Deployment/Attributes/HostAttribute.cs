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
        /// <summary>
        /// Hostname (or alias), as a string.
        /// </summary>
        public string Host { get; private set; }

        /// <summary>
        /// Defines a host that the task should run on.
        /// The parameter may be either a hostname or
        /// an alias and the attribute may be used
        /// multiple times on a single Task.
        /// </summary>
        /// <param name="hostNameOrAlias">
        ///     Hostname or Alias that the task should run on.
        /// </param>
        public HostAttribute(string hostNameOrAlias)
        {
            Host = hostNameOrAlias;
        }
    }
}
