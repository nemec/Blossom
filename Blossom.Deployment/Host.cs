using System;
using System.Text;
using System.Xml.Serialization;

namespace Blossom.Deployment
{
    [XmlRoot("Host")]
    public class Host : IEquatable<Host>
    {
        public const string LoopbackHostname = "loopback";

        [XmlText]
        public string Hostname { get; set; }

        [XmlAttribute("alias")]
        public string Alias { get; set; }

        [XmlAttribute("roles")]
        public string Roles { get; set; }

        [XmlAttribute("username")]
        public string Username { get; set; }

        [XmlAttribute("password")]
        public string Password { get; set; }

        [XmlAttribute("port")]
        public int Port { get; set; }

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