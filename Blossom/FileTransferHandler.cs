using Blossom.Logging;
using System;

namespace Blossom
{
    /// <summary>
    /// A class that manages the progress of file transfers.
    /// </summary>
    public class FileTransferHandler : IFileTransferHandler
    {
        /// <summary>
        /// Name of the file being transferred.
        /// </summary>
        public string Filename { get; set; }

        private ILogger Logger { get; set; }

        /// <summary>
        /// Total number of bytes transferred.
        /// </summary>
        public ulong BytesTransferred { get; private set; }

        private int ElapsedTime { get; set; }

        private readonly System.Timers.Timer _timer;
        
        /// <summary>
        /// Create a new handler to manage the progress of file transfers.
        /// </summary>
        /// <param name="logger"></param>
        public FileTransferHandler(ILogger logger)
        {
            Logger = logger;

            _timer = new System.Timers.Timer(1000);
            _timer.Elapsed += TimerElapsed;
            _timer.Start();
        }

        /// <summary>
        /// </summary>
        ~FileTransferHandler()
        {
            Dispose(false);
        }

        /// <summary>
        /// Increment the number of bytes transferred.
        /// </summary>
        /// <param name="bytes"></param>
        public void IncrementBytesTransferred(ulong bytes)
        {
            BytesTransferred += bytes;
            Logger.Tick(String.Format("Transferring {0}... {1} [{2}s]",
                Filename ?? "<Unknown>", HumanizeBytes(BytesTransferred), ElapsedTime));
        }

        /// <summary>
        /// Log that the file in question does not exist.
        /// </summary>
        public void FileDoesNotExist()
        {
            Logger.Error(String.Format("File {0} does not exist.", Filename ?? "<Unknown>"));
        }

        /// <summary>
        /// Log that the file has not been transferred because it
        /// already exists (and has the same contents).
        /// </summary>
        public void FileUpToDate()
        {
            Logger.Info(String.Format("{0} already up to date. Not copying.", Filename ?? "<Unknown>"));
        }

        /// <summary>
        /// Log that the file transfer was canceled.
        /// </summary>
        public void TransferCanceled()
        {
            Logger.Error(String.Format("Transfer of file {0} was canceled.", Filename ?? "<Unknown>"));
        }

        /// <summary>
        /// Log that the file transfer was completed.
        /// </summary>
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

        /// <summary>
        /// 
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="disposeManaged"></param>
        protected virtual void Dispose(bool disposeManaged)
        {
            if (disposeManaged)
            {
                _timer.Dispose();
            }
        }
    }
}
