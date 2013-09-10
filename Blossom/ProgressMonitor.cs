using Blossom.Deployment.Logging;
using System;
using Tamir.SharpSsh.jsch;

namespace Blossom.Deployment
{
    public class FileProgressMonitor : SftpProgressMonitor, IDisposable
    {
        private string Filename { get; set; }

        private ILogger Logger { get; set; }

        private long Max { get; set; }

        private long Count { get; set; }

        private long Percent { get; set; }

        private int ElapsedTime { get; set; }

        private System.Timers.Timer Timer;

        public FileProgressMonitor(ILogger logger, string filename)
        {
            Filename = filename;
            Logger = logger;
        }

        ~FileProgressMonitor()
        {
            Dispose(false);
        }

        public override void init(int op, string src, string dest, long max)
        {
            Max = max;
            Timer = new System.Timers.Timer(1000);
            Timer.Elapsed += TimerElapsed;
            Timer.Start();
        }

        public override bool count(long count)
        {
            Count += count;
            if (Percent < count * 100 / Max)
            {
                Percent = Count * 100 / Max;

                Logger.Tick(String.Format("Transferring {0}... {1}/{2} [{3}s]",
                    Filename, Utils.HumanizeBytes(Count),
                    Utils.HumanizeBytes(Max), ElapsedTime));
            }
            return true;
        }

        public override void end()
        {
            Timer.Stop();
            Timer.Dispose();
            Logger.Info(String.Format("Completed {0}...... {1} [{2}s]",
                Filename, Utils.HumanizeBytes(Max), ElapsedTime));
            Logger.ClearTicker();
        }

        private void TimerElapsed(object sender, System.Timers.ElapsedEventArgs args)
        {
            ElapsedTime++;
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