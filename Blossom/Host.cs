using System.Text;
using Blossom.Environments;

namespace Blossom
{
    /// <summary>
    /// Description of a remote host.
    /// </summary>
    public class Host : IHost
    {
        /// <summary>
        /// Constant value of the "loopback" remote host. This
        /// host, when specified, will bypass the network execute
        /// all operations locally.
        /// </summary>
        public const string LoopbackHostname = "loopback";

        /// <inheritdoc />
        public string Hostname { get; set; }

        /// <inheritdoc />
        public string Alias { get; set; }

        /// <inheritdoc />
        public string Roles { get; set; }

        /// <inheritdoc />
        public string Username { get; set; }

        /// <inheritdoc />
        public string Password { get; set; }

        /// <inheritdoc />
        public int Port { get; set; }

        /// <inheritdoc />
        public IEnvironment Environment { get; set; }

        /// <summary>
        /// Create a new host using the hostname "localhost",
        /// the username of the current user, no password, and
        /// a port of 22.
        /// </summary>
        public Host()
        {
            Hostname = "localhost";
            Username = System.Environment.UserName;
            Port = 22;
            Environment = new Linux();
        }

        /// <inheritdoc />
        /// <returns></returns>
        public override string ToString()
        {
            var builder = new StringBuilder();
            if (Username != null)
            {
                builder.Append(Username);
                builder.Append("@");
            }
            builder.Append(Hostname);
            if (Port != 0)
            {
                builder.Append(":");
                builder.Append(Port);
            }
            return builder.ToString();
        }
    }
}