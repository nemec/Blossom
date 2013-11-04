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
        string Hostname { get; set; }

        /// <summary>
        /// An optional alias for this host. Accepted as an alternative
        /// to specifying the hostname during task assignment.
        /// </summary>
        string Alias { get; set; }

        /// <summary>
        /// Semicolon-delimited list of roles.
        /// </summary>
        string Roles { get; set; }

        /// <summary>
        /// Login username for the host.
        /// </summary>
        string Username { get; set; }

        /// <summary>
        /// Login password for the host.
        /// </summary>
        string Password { get; set; }

        /// <summary>
        /// Host's SSH port. Defaults to 22.
        /// </summary>
        int Port { get; set; }

        /// <summary>
        /// Host's operating system environment. Determines platform-specific
        /// data like path-combine semantics.
        /// </summary>
        IEnvironment Environment { get; set; }
    }
}
