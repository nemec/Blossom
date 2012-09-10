using Tamir.SharpSsh.jsch;

namespace Blossom.Deployment.Ssh
{
    public interface ISftp
    {
        bool Connected { get; }

        void Connect();

        void Disconnect();

        void Mkdir(string path);

        SftpATTRS Stat(string path);

        void Put(string source, string destination, FileProgressMonitor monitor);
    }
}