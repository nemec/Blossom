using System;

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

        void Debug(string message, Exception exception = null);

        void Verbose(string message, Exception exception = null);

        void Info(string message, Exception exception = null);

        void Warn(string message, Exception exception = null);

        void Error(string message, Exception exception = null);

        /// <summary>
        /// A fatal error happened. This usually means the
        /// application is unrecoverable.
        /// </summary>
        /// <param name="message">Error message to be displayed.</param>
        /// <param name="exception">Exception associated with the error.</param>
        void Fatal(string message, Exception exception = null);
    }
}