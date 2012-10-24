using System;
using Blossom.Deployment.Exceptions;

namespace Blossom.Deployment.Logging
{
    public class NullLogger : ILogger
    {
        public LogLevel DisplayLogLevel { get; set; }

        public LogLevel AbortLogLevel { get; set; }

        public IDeploymentContext Context { get; set; }

        public void Tick(string message)
        {
        }

        public void ClearTicker()
        {
        }

        public void Debug(string message, Exception exception = null)
        {
            if(AbortLogLevel <= LogLevel.Debug)
            {
                throw new AbortExecutionException();
            }
        }

        public void Verbose(string message, Exception exception = null)
        {
            if (AbortLogLevel <= LogLevel.Verbose)
            {
                throw new AbortExecutionException();
            }
        }

        public void Info(string message, Exception exception = null)
        {
            if (AbortLogLevel <= LogLevel.Info)
            {
                throw new AbortExecutionException();
            }
        }

        public void Warn(string message, Exception exception = null)
        {
            if (AbortLogLevel <= LogLevel.Warn)
            {
                throw new AbortExecutionException();
            }
        }

        public void Error(string message, Exception exception = null)
        {
            if (AbortLogLevel <= LogLevel.Error)
            {
                throw new AbortExecutionException();
            }
        }

        public void Fatal(string message, Exception exception = null)
        {
            if (AbortLogLevel <= LogLevel.Fatal)
            {
                throw new AbortExecutionException();
            }
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
