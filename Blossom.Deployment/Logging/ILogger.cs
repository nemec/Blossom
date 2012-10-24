using System;
using Blossom.Deployment.Exceptions;

namespace Blossom.Deployment.Logging
{
    public interface ILogger
    {
        /// <summary>
        /// Allow access to the IDeploymentContext during logging.
        /// </summary>
        IDeploymentContext Context { get; set; }

        /// <summary>
        /// Define the minimum log level that should be recorded.
        /// All logging to levels less than the given is ignored.
        /// </summary>
        LogLevel DisplayLogLevel { get; set; }

        /// <summary>
        /// Define the minimum log level to abort execution.
        /// Attempts to log at this level or above should
        /// throw an <exception cref="AbortExecutionException"></exception>
        /// </summary>
        LogLevel AbortLogLevel { get; set; }

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