using Blossom.Deployment;
using Blossom.Deployment.Logging;
using System;

namespace Blossom.Examples.Compression
{
    public class FileTransferHandler : IFileTransferHandler
    {
        public string Filename { get; set; }

        private ILogger Logger { get; set; }

        public ulong BytesTransferred { get; private set; }

        private int ElapsedTime { get; set; }

        private readonly System.Timers.Timer _timer;
        
        public FileTransferHandler(ILogger logger)
        {
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
                Filename ?? "<Unknown>", HumanizeBytes(BytesTransferred), ElapsedTime));
        }
        public void FileDoesNotExist()
        {
            Logger.Error(String.Format("File {0} does not exist.", Filename ?? "<Unknown>"));
        }

        public void FileUpToDate()
        {
            Logger.Info(String.Format("{0} already up to date. Not copying.", Filename ?? "<Unknown>"));
        }

        public void TransferCanceled()
        {
            Logger.Error(String.Format("Transfer of file {0} was canceled.", Filename ?? "<Unknown>"));
        }

        public void TransferCompleted()
        {
            _timer.Stop();
            _timer.Dispose();
            Logger.Info(String.Format("Copied {0}...... {1} [{2}s]",
                Filename ?? "<Unknown>", HumanizeBytes(BytesTransferred), ElapsedTime));
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
