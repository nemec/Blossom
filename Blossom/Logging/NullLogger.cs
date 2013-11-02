using System;
using Blossom.Exceptions;

namespace Blossom.Logging
{
    /// <summary>
    /// Logger that displays nothing.
    /// </summary>
    public class NullLogger : ILogger
    {
        /// <inheritdoc />
        public LogLevel DisplayLogLevel { get; set; }

        /// <inheritdoc />
        public LogLevel AbortLogLevel { get; set; }

        /// <inheritdoc />
        public void Tick(string message)
        {
        }

        /// <inheritdoc />
        public void ClearTicker()
        {
        }

        /// <inheritdoc />
        public void Log(LogLevel level, string message, Exception exception = null)
        {
            if (AbortLogLevel <= level)
            {
                throw new AbortExecutionException();
            }
        }

        /// <inheritdoc />
        public void Debug(string message, Exception exception = null)
        {
            Log(LogLevel.Debug, message, exception);
        }

        /// <inheritdoc />
        public void Verbose(string message, Exception exception = null)
        {
            Log(LogLevel.Verbose, message, exception);
        }

        /// <inheritdoc />
        public void Info(string message, Exception exception = null)
        {
            Log(LogLevel.Info, message, exception);
        }

        /// <inheritdoc />
        public void Warn(string message, Exception exception = null)
        {
            Log(LogLevel.Warn, message, exception);
        }

        /// <inheritdoc />
        public void Error(string message, Exception exception = null)
        {
            Log(LogLevel.Error, message, exception);
        }

        /// <inheritdoc />
        public void Fatal(string message, Exception exception = null)
        {
            Log(LogLevel.Fatal, message, exception);
        }

        /// <inheritdoc />
        public void Abort(string message, Exception exception = null)
        {
            if (exception is AbortExecutionException)
            {
                throw exception;
            }
            throw new AbortExecutionException(message, exception);
        }
    }
}
