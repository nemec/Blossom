using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blossom.Deployment.Logging
{
    public class NullLogger : ILogger
    {
        public void Tick(string message)
        {
        }

        public void ClearTicker()
        {
        }

        public void Debug(string message, Exception exception = null)
        {
        }

        public void Verbose(string message, Exception exception = null)
        {
        }

        public void Info(string message, Exception exception = null)
        {
        }

        public void Warn(string message, Exception exception = null)
        {
        }

        public void Error(string message, Exception exception = null)
        {
        }

        public void Fatal(string message, Exception exception = null)
        {
        }
    }
}
