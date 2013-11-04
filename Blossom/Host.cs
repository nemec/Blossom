using System;
using System.Collections.Generic;
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
        public IEnumerable<string> Roles { get; set; }

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

        /// <summary>
        /// Create a new hostname from the input "host string".
        /// Able to specify username, hostname, and port with this format:
        ///     user@host:port
        /// </summary>
        /// <param name="hostStr"></param>
        public Host(string hostStr)
            : this()
        {
            var hostnameIdx = hostStr.LastIndexOf('@');
            if (hostnameIdx > 0)
            {
                Username = hostStr.Substring(0, hostnameIdx);
                hostStr = hostStr.Substring(hostnameIdx + 1);
            }

            // IPv6
            if (hostStr.Split(':').Length > 2)  // More than one :
            {
                var portIdx = hostStr.LastIndexOf(':');
                var ipv6EndBracketIdx = hostStr.LastIndexOf(']');
                if (ipv6EndBracketIdx >= 0)
                {
                    if (!hostStr.StartsWith("["))
                    {
                        throw new FormatException(String.Format(
                            "IPv6 address {0} must be wrapped in brackets.", hostStr));
                    }
                    Hostname = hostStr.Substring(1, ipv6EndBracketIdx - 1);

                    if (portIdx > ipv6EndBracketIdx)
                    {
                        Port = Int32.Parse(hostStr.Substring(portIdx + 1));
                    }
                }
                else
                {
                    Hostname = hostStr;
                }
            }
            else
            {
                var portIdx = hostStr.IndexOf(':');
                if (portIdx > 0)
                {
                    Hostname = hostStr.Substring(0, portIdx);
                    Port = Int32.Parse(hostStr.Substring(portIdx + 1));
                }
                else
                {
                    Hostname = hostStr;
                }
            }

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