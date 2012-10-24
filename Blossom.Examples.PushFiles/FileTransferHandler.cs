using Blossom.Deployment.Logging;
using Renci.SshNet.Sftp;
using System;

namespace Blossom.Examples.PushFiles
{
    public class FileTransferHandler : IFileTransferHandler
    {
        private string Filename { get; set; }

        private ILogger Logger { get; set; }

        public ulong BytesTransferred { get; private set; }

        private int ElapsedTime { get; set; }

        private readonly System.Timers.Timer _timer;
        
        public FileTransferHandler(ILogger logger, string filename)
        {
            Filename = filename;
            Logger = logger;

            _timer = new System.Timers.Timer(1000);
            _timer.Elapsed += TimerElapsed;
            _timer.Start();
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
            _timer.Stop();
            _timer.Dispose();
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
            const int byteRadix = 1000;
            foreach (var suffix in new[] { "B", "kB", "MB", "GB", "TB" })
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
                _timer.Dispose();
            }
        }
    }
}
