using System;
using Blossom.Exceptions;

namespace Blossom.Logging
{
    /// <summary>
    /// Allow logging of informational and error messages.
    /// </summary>
    public interface ILogger
    {
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

        /// <summary>
        /// Clear the ticker from the screen.
        /// </summary>
        void ClearTicker();

        /// <summary>
        /// Log a message with a custom <see cref="LogLevel"/>
        /// </summary>
        /// <param name="level">Logging level.</param>
        /// <param name="message">Message to log.</param>
        /// <param name="exception">Optional exception to attach to the log.</param>
        void Log(LogLevel level, string message, Exception exception = null);

        /// <summary>
        /// Log a debugging message.
        /// </summary>
        /// <param name="message">Message to log.</param>
        /// <param name="exception">Optional exception to attach to the log.</param>
        void Debug(string message, Exception exception = null);

        /// <summary>
        /// Log a verbose message.
        /// </summary>
        /// <param name="message">Message to log.</param>
        /// <param name="exception">Optional exception to attach to the log.</param>
        void Verbose(string message, Exception exception = null);

        /// <summary>
        /// Log an informational message.
        /// </summary>
        /// <param name="message">Message to log.</param>
        /// <param name="exception">Optional exception to attach to the log.</param>
        void Info(string message, Exception exception = null);

        /// <summary>
        /// Log a warning message.
        /// </summary>
        /// <param name="message">Message to log.</param>
        /// <param name="exception">Optional exception to attach to the log.</param>
        void Warn(string message, Exception exception = null);

        /// <summary>
        /// Log an error message.
        /// </summary>
        /// <param name="message">Message to log.</param>
        /// <param name="exception">Optional exception to attach to the log.</param>
        void Error(string message, Exception exception = null);

        /// <summary>
        /// A fatal error happened. This usually means the
        /// application is unrecoverable.
        /// </summary>
        /// <param name="message">Error message to be displayed.</param>
        /// <param name="exception">Exception associated with the error.</param>
        void Fatal(string message, Exception exception = null);

        /// <summary>
        /// Immediately display an error message and abort.
        /// </summary>
        /// <param name="message"></param>
        /// <param name="exception"></param>
        void Abort(string message, Exception exception = null);
    }
}