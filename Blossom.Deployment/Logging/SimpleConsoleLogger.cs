using System;
using Blossom.Deployment.Exceptions;

namespace Blossom.Deployment.Logging
{
    /// <summary>
    /// Simple threadsafe logger implementation that
    /// prints to the console.
    /// </summary>
    public class SimpleConsoleLogger : ILogger
    {
        private readonly object _lock;

        public LogLevel DisplayLogLevel { get; set; }

        public LogLevel AbortLogLevel { get; set; }

        protected string TitleSeparator = ": ";

        protected string LogTitle = "Log";
        protected string DebugTitle = "Debug";
        protected string VerboseTitle = "Verbose";
        protected string InfoTitle = "Info";
        protected string WarnTitle = "Warn";
        protected string ErrorTitle = "Error";
        protected string FatalTitle = "Fatal";
        protected string AbortTitle = "Abort";

        /// <summary>
        /// Create a default logger with the <see cref="AbortLogLevel"/>
        /// set to <see cref="LogLevel.Fatal"/>.
        /// </summary>
        public SimpleConsoleLogger()
        {
            _lock = new object();
            AbortLogLevel = LogLevel.Fatal;
        }

        public virtual void Tick(string message)
        {
            var currentCol = Console.CursorLeft;
            var currentLine = Console.CursorTop;

            message = message.Length > Console.WindowWidth - 1 ?
                message.Substring(0, Math.Min(message.Length, Console.WindowWidth - 1)) :
                message.PadRight(Console.WindowWidth - 1);

            lock (_lock)
            {
                Console.SetCursorPosition(0, Console.WindowTop + Console.WindowHeight - 1);
                Console.Write(message);
                Console.SetCursorPosition(currentCol, currentLine);
            }
        }

        public virtual void ClearTicker()
        {
            var currentCol = Console.CursorLeft;
            var currentLine = Console.CursorTop;

            lock (_lock)
            {
                Console.SetCursorPosition(0, Console.WindowTop + Console.WindowHeight - 1);
                Console.Write("".PadRight(Console.WindowWidth - 1));
                Console.SetCursorPosition(currentCol, currentLine);
            }
        }

        // TODO: http://www.dotnetperls.com/console-color
        protected virtual void WriteLog(
            LogLevel level, string title, string message, Exception exception = null)
        {
            Console.WriteLine(title + TitleSeparator + message);
            if (exception != null)
            {
                Console.Error.WriteLine("\t" + exception.Message);
            }
        }

        private void Log(LogLevel level, string title, string message, Exception exception)
        {
            lock (_lock)
            {
                // Abort on messages with the same importance (or greater) than `level`
                if (level >= AbortLogLevel)
                {
                    throw new AbortExecutionException(message, exception);
                }
                // Don't display messages less important than `level`
                if (level < DisplayLogLevel) return;

                WriteLog(level, title, message, exception);
            }
        }

        public void Abort(string message, Exception exception)
        {
            lock (_lock)
            {
                if (exception is AbortExecutionException)
                {
                    WriteLog(LogLevel.Fatal, AbortTitle, exception.Message);
                    throw exception;
                }
                
                WriteLog(LogLevel.Fatal, AbortTitle, message, exception);
                throw new AbortExecutionException(message, exception);
            }
        }

        #region Log levels

        public void Log(LogLevel level, string message, Exception exception)
        {
            Log(level, LogTitle, message, exception);
        }

        public void Debug(string message, Exception exception)
        {
            Log(LogLevel.Debug, DebugTitle, message, exception);
        }

        public void Verbose(string message, Exception exception)
        {
            Log(LogLevel.Verbose, VerboseTitle, message, exception);
        }

        public void Info(string message, Exception exception)
        {
            Log(LogLevel.Info, InfoTitle, message, exception);
        }

        public void Warn(string message, Exception exception)
        {
            Log(LogLevel.Warn, WarnTitle, message, exception);
        }
        public void Error(string message, Exception exception)
        {
            Log(LogLevel.Error, ErrorTitle, message, exception);
        }

        public void Fatal(string message, Exception exception)
        {
            Log(LogLevel.Fatal, FatalTitle, message, exception);
        }

        #endregion
    }
}