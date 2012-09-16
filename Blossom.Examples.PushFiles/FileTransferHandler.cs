using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Blossom.Deployment;
using Blossom.Deployment.Logging;
using Blossom.Deployment.Ssh;
using Renci.SshNet.Sftp;

namespace Blossom.Examples.PushFiles
{
    public class FileTransferHandler : IFileTransferHandler
    {
        private string Filename { get; set; }

        private ILogger Logger { get; set; }

        public ulong BytesTransferred { get; private set; }

        private int ElapsedTime { get; set; }

        private System.Timers.Timer Timer;
        
        public FileTransferHandler(ILogger logger, string filename)
        {
            Filename = filename;
            Logger = logger;

            Timer = new System.Timers.Timer(1000);
            Timer.Elapsed += TimerElapsed;
            Timer.Start();
        }

        ~FileTransferHandler()
        {
            Dispose(false);
        }

        public void IncrementBytesTransferred(ulong bytes)
        {
            BytesTransferred += bytes;
            Logger.Tick(String.Format("Transferring {0}... {1} [{2}s]",
                Filename, HumanizeBytes(BytesTransferred), ElapsedTime));
        }

        public void TransferCompleted()
        {
            Timer.Stop();
            Timer.Dispose();
            Logger.Info(String.Format("Completed {0}...... {1} [{2}s]",
                Filename, HumanizeBytes(BytesTransferred), ElapsedTime));
            Logger.ClearTicker();
        }

        private void TimerElapsed(object sender, System.Timers.ElapsedEventArgs args)
        {
            ElapsedTime++;
        }

        private static string HumanizeBytes(ulong bytes)
        {
            double tmpbytes = bytes;
            int byteRadix = 1000;
            foreach (var suffix in new string[] { "B", "kB", "MB", "GB", "TB" })
            {
                if (tmpbytes < byteRadix)
                {
                    return String.Format("{0:0.##}{1}", tmpbytes, suffix);
                }
                tmpbytes = tmpbytes / byteRadix;
            }
            return String.Format("{0}B", bytes);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposeManaged)
        {
            if (disposeManaged)
            {
                Timer.Dispose();
            }
        }
    }
}
