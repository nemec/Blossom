using System;
using Blossom.Deployment.Exceptions;

namespace Blossom.Deployment.Logging
{
    /// <summary>
    /// Logger that displays nothing.
    /// </summary>
    public class NullLogger : ILogger
    {
        public LogLevel DisplayLogLevel { get; set; }

        public LogLevel AbortLogLevel { get; set; }

        public void Tick(string message)
        {
        }

        public void ClearTicker()
        {
        }

        public void Log(LogLevel level, string message, Exception exception = null)
        {
            if (AbortLogLevel <= level)
            {
                throw new AbortExecutionException();
            }
        }

        public void Debug(string message, Exception exception = null)
        {
            Log(LogLevel.Debug, message, exception);
        }

        public void Verbose(string message, Exception exception = null)
        {
            Log(LogLevel.Verbose, message, exception);
        }

        public void Info(string message, Exception exception = null)
        {
            Log(LogLevel.Info, message, exception);
        }

        public void Warn(string message, Exception exception = null)
        {
            Log(LogLevel.Warn, message, exception);
        }

        public void Error(string message, Exception exception = null)
        {
            Log(LogLevel.Error, message, exception);
        }

        public void Fatal(string message, Exception exception = null)
        {
            Log(LogLevel.Fatal, message, exception);
        }


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
