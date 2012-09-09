using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Blossom.Deployment
{
    [XmlRoot("Host")]
    public class Host
    {
        [XmlText]
        public string Hostname { get; set; }

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
    }
}
