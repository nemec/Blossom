using System.IO;
using Tamir.SharpSsh.jsch;

namespace Blossom.Deployment.Ssh
{
    public class SftpWrapper : ISftp
    {
        private ChannelSftp Sftp { get; set; }

        public bool Connected { get { return Sftp != null && Sftp.isConnected(); } }

        public SftpWrapper(ISession session)
        {
            Sftp = session.GetChannel<ChannelSftp>();
        }

        public void Connect()
        {
            Sftp.connect();
        }

        public void Disconnect()
        {
            Sftp.disconnect();
        }

        public void Mkdir(string path)
        {
            Sftp.mkdir(path);
        }

        public SftpATTRS Stat(string path)
        {
            try
            {
                return Sftp.stat(path);
            }
            catch (SftpException exception)
            {
                if (exception.id == ChannelSftp.SSH_FX_NO_SUCH_FILE)
                {
                    throw new FileNotFoundException(path, exception);
                }
                else
                {
                    throw;
                }
            }
        }

        public void Put(string source, string destination, FileProgressMonitor monitor)
        {
            Sftp.put(source, destination, monitor, ChannelSftp.OVERWRITE);
        }
    }
}