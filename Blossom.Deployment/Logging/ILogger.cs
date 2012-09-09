using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blossom.Deployment.Logging
{
    public interface ILogger
    {
        /// <summary>
        /// Display text in a ticker at the bottom
        /// of the logging interface. If there is no
        /// appropriate interface, leave it as no-op
        /// otherwise the logger may fill with
        /// ticker messages.
        /// </summary>
        /// <param name="message">Text to display</param>
        void Tick(string message);

        void ClearTicker();

        void Debug(string message);
        
        void Info(string message);

        void Warn(string message);
        void Warn(string message, Exception exception);

        void Error(string message);
        void Error(string message, Exception exception);

        void Fatal(string message);
        void Fatal(string message, Exception exception);
    }
}
