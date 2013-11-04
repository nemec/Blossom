using System;
using System.Linq;
using System.Collections.Generic;
using Blossom;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DeploymentUnitTest
{
    [TestClass]
    public class HostUnitTest
    {
        private class HostEqualityComparer : IEqualityComparer<Host>
        {
            public bool Equals(Host x, Host y)
            {
                return x.Username == y.Username &&
                       x.Password == y.Password &&
                       x.Hostname == y.Hostname &&
                       x.Port == y.Port &&
                       (x.Roles ?? new string[0]).OrderBy(r => r)
                            .SequenceEqual((y.Roles ?? new string[0]).OrderBy(r => r));

            }

            public int GetHashCode(Host obj)
            {
                return (obj.Hostname ?? "").GetHashCode() ^
                       (obj.Password ?? "").GetHashCode() ^
                       (obj.Username ?? "").GetHashCode() ^
                       String.Join("", 
                            obj.Roles ?? new string[0]).GetHashCode() ^
                       obj.Port.GetHashCode();
            }
        }

        [TestMethod]
        public void CreateFromHostString_WithUsernameHostnameAndPort_CreatesValidHost()
        {
            const string hostStr = "admin@foo.com:222";
            var expected = new Host
                {
                    Hostname = "foo.com",
                    Port = 222,
                    Username = "admin"
                };

            var host = new Host(hostStr);

            Assert.IsTrue(new HostEqualityComparer().Equals(expected, host));
        }

        [TestMethod]
        public void CreateFromHostString_WithUsernameAndHostname_CreatesValidHost()
        {
            const string hostStr = "deploy@website";
            var expected = new Host
                {
                    Hostname = "website",
                    Username = "deploy"
                };

            var host = new Host(hostStr);

            Assert.IsTrue(new HostEqualityComparer().Equals(expected, host));
        }

        [TestMethod]
        public void CreateFromHostString_WithHostname_CreatesValidHost()
        {
            const string hostStr = "nameserver1";
            var expected = new Host
                {
                    Hostname = "nameserver1",
                };

            var host = new Host(hostStr);

            Assert.IsTrue(new HostEqualityComparer().Equals(expected, host));
        }

        [TestMethod]
        public void CreateFromHostString_WithIpv6Hostname_CreatesValidHost()
        {
            const string hostStr = "::1";
            var expected = new Host
                {
                    Hostname = "::1",
                };

            var host = new Host(hostStr);

            Assert.IsTrue(new HostEqualityComparer().Equals(expected, host));
        }

        [TestMethod]
        public void CreateFromHostString_WithBracketedIpv6Hostname_CreatesValidHost()
        {
            const string hostStr = "[::1]";
            var expected = new Host
                {
                    Hostname = "::1"
                };

            var host = new Host(hostStr);

            Assert.IsTrue(new HostEqualityComparer().Equals(expected, host));
        }

        [TestMethod]
        public void CreateFromHostString_WithBracketedIpv6HostnameAndPort_CreatesValidHost()
        {
            const string hostStr = "[::1]:1222";
            var expected = new Host
                {
                    Hostname = "::1",
                    Port = 1222
                };

            var host = new Host(hostStr);

            Assert.IsTrue(new HostEqualityComparer().Equals(expected, host));
        }

        [TestMethod]
        public void CreateFromHostString_WithUsernameAndIpv6Hostname_CreatesValidHost()
        {
            const string hostStr = "user@2001:db8::1";
            var expected = new Host
                {
                    Username = "user",
                    Hostname = "2001:db8::1"
                };

            var host = new Host(hostStr);

            Assert.IsTrue(new HostEqualityComparer().Equals(expected, host));
        }

        [TestMethod]
        public void CreateFromHostString_WithUsernameAndIpv6HostnameAndPort_CreatesValidHost()
        {
            const string hostStr = "user@[2001:db8::1]:1222";
            var expected = new Host
                {
                    Username = "user",
                    Hostname = "2001:db8::1",
                    Port = 1222
                };

            var host = new Host(hostStr);

            Assert.IsTrue(new HostEqualityComparer().Equals(expected, host));
        }

        [TestMethod]
        public void CreateFromHostString_WithUsernameContainingAtSymbol_CreatesValidHost()
        {
            const string hostStr = "user@google.com@myserver:222";
            var expected = new Host
                {
                    Username = "user@google.com",
                    Hostname = "myserver",
                    Port = 222
                };

            var host = new Host(hostStr);

            Assert.IsTrue(new HostEqualityComparer().Equals(expected, host));
        }
    }
}
