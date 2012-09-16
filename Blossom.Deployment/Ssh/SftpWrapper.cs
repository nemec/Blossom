using Renci.SshNet;
using Renci.SshNet.Common;
using Renci.SshNet.Sftp;
using System;
using System.Collections;
using System.Collections.Generic;
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
            Sftp.BufferSize = 1024 * 16;
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

        public IEnumerable<string> ListDirectory(string path)
        {
            foreach (var file in Sftp.ListDirectory(path))
            {
                yield return file.Name;
            }
        }

        public void Get(string source, string destination, IFileTransferHandler handler)
        {
            using (var file = File.OpenWrite(destination))
            {
                var result = Sftp.BeginDownloadFile(source, file, handler, new object());
                Sftp.EndDownloadFile(result);
            }

        }

        public bool Put(string source, string destination, IFileTransferHandler handler)
        {
            return Put(source, destination, handler, false);
        }

        public bool Put(string source, string destination, IFileTransferHandler handler, bool ifNewer)
        {
            if (ifNewer)
            {
                var modified = File.GetLastWriteTimeUtc(source);
                var remoteModified = Sftp.GetLastWriteTimeUtc(destination);
                if (modified <= remoteModified)
                {
                    return false;
                }
            }
            using (var file = File.OpenRead(source))
            {
                Put(file, destination, handler);
            }
            return true;
        }

        public void Put(Stream stream, string destination)
        {
            Sftp.UploadFile(stream, destination, true);
        }

        public void Put(Stream stream, string destination, IFileTransferHandler handler)
        {
            var result = Sftp.BeginUploadFile(stream, destination, handler, new object());
            Sftp.EndUploadFile(result);
        }
    }
}