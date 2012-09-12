using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Renci.SshNet;
using System.IO;

namespace Blossom.Deployment.Ssh
{
    public class ShellWrapper : IShell, IDisposable
    {
        private SshClient Client { get; set; }

        public Stream Stream { get; private set; }

        public ShellWrapper(Host host)
        {
            Client = new SshClient(host.Hostname, host.Username, host.Password);
            Client.Connect();
            Stream = Client.CreateShellStream(String.Format("{0}@{1}", host.Username, host.Hostname), 0, 0, 0, 0, 1024);
        }

        ~ShellWrapper()
        {
            Dispose(false);
        }

        public string RunCommand(string command)
        {
            return Client.RunCommand(command).Result;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected void Dispose(bool disposeManaged)
        {
            if (Client != null && Client.IsConnected)
            {
                Client.Disconnect();
            }

            if (disposeManaged && Client != null)
            {
                Client.Dispose();
            }
        }
    }
}
