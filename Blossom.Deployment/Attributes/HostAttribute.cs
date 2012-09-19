using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blossom.Deployment
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
