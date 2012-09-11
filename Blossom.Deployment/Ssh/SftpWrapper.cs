using Renci.SshNet;
using System;
using System.IO;

namespace Blossom.Deployment.Ssh
{
    public class SftpWrapper : ISftp
    {
        private SftpClient Sftp { get; set; }

        public bool IsConnected { get { return Sftp != null && Sftp.IsConnected; } }

        public SftpWrapper(Host host)
        {
            Sftp = new SftpClient(host.Hostname, host.Username, host.Password);
            Sftp.OperationTimeout = TimeSpan.FromMinutes(8);
            Sftp.ConnectionInfo.Timeout = TimeSpan.FromMinutes(2);
        }

        public void Connect()
        {
            Sftp.Connect();
        }

        public void Disconnect()
        {
            Sftp.Disconnect();
        }

        public void Mkdir(string path)
        {
            Sftp.CreateDirectory(path);
        }

        public bool Exists(string path)
        {
            return Sftp.Exists(path);
        }

        public void Put(string source, string destination)
        {
            using (var file = File.OpenRead(source))
            {
                Sftp.UploadFile(file, destination, true);
            }
        }
    }
}