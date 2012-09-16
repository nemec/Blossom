using Renci.SshNet.Sftp;
using System.Collections.Generic;
using System.IO;

namespace Blossom.Deployment.Ssh
{
    public interface ISftp
    {
        bool IsConnected { get; }

        void Connect();

        void Disconnect();

        void Mkdir(string path);

        bool Exists(string path);

        void Get(string source, string destination, IFileTransferHandler handler);

        bool Put(string source, string destination, IFileTransferHandler handler);

        bool Put(string source, string destination, IFileTransferHandler handler, bool ifNewer);

        void Put(Stream source, string destination);

        void Put(Stream source, string destination, IFileTransferHandler handler);
    }
}