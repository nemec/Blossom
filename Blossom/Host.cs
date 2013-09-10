using System;
using System.Text;
using System.Xml.Serialization;

namespace Blossom
{
    /// <summary>
    /// Description of a remote host.
    /// </summary>
    [XmlRoot("Host")]
    public class Host : IEquatable<Host>
    {
        /// <summary>
        /// Constant value of the "loopback" remote host. This
        /// host, when specified, will bypass the network execute
        /// all operations locally.
        /// </summary>
        public const string LoopbackHostname = "loopback";

        /// <summary>
        /// Hostname or IP address of the remote host.
        /// </summary>
        [XmlText]
        public string Hostname { get; set; }

        /// <summary>
        /// An optional alias for this host. Accepted as an alternative
        /// to specifying the hostname during task assignment.
        /// </summary>
        [XmlAttribute("alias")]
        public string Alias { get; set; }

        /// <summary>
        /// Semicolon-delimited list of roles.
        /// </summary>
        [XmlAttribute("roles")]
        public string Roles { get; set; }

        /// <summary>
        /// Login username for the host.
        /// </summary>
        [XmlAttribute("username")]
        public string Username { get; set; }

        /// <summary>
        /// Login password for the host.
        /// </summary>
        [XmlAttribute("password")]
        public string Password { get; set; }

        /// <summary>
        /// Host's SSH port. Defaults to 22.
        /// </summary>
        [XmlAttribute("port")]
        public int Port { get; set; }

        /// <summary>
        /// Create a new host using the hostname "localhost",
        /// the username of the current user, no password, and
        /// a port of 22.
        /// </summary>
        public Host()
        {
            Hostname = "localhost";
            Username = Environment.UserName;
            Port = 22;
        }

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

        public bool Equals(Host other)
        {
            return other != null &&
                Hostname == other.Hostname &&
                Port == other.Port;
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as Host);
        }

        public override int GetHashCode()
        {
            var num = -1962473570;
            num = num * Hostname.GetHashCode();
            num = num * Port;
            return num;
        }
    }
}