using System.Collections.Generic;
using Blossom.Environments;

namespace Blossom
{
    /// <summary>
    /// Defines a set of properties representing a Host
    /// </summary>
    public interface IHost
    {
        /// <summary>
        /// Hostname or IP address of the remote host.
        /// </summary>
        string Hostname { get; }

        /// <summary>
        /// An optional alias for this host. Accepted as an alternative
        /// to specifying the hostname during task assignment.
        /// </summary>
        string Alias { get; }

        /// <summary>
        /// Semicolon-delimited list of roles.
        /// </summary>
        IEnumerable<string> Roles { get; }

        /// <summary>
        /// Login username for the host.
        /// </summary>
        string Username { get; }

        /// <summary>
        /// Login password for the host.
        /// </summary>
        string Password { get; }

        /// <summary>
        /// Host's SSH port. Defaults to 22.
        /// </summary>
        int Port { get; }

        /// <summary>
        /// Host's operating system environment. Determines platform-specific
        /// data like path-combine semantics.
        /// </summary>
        IEnvironment Environment { get; }
    }
}
