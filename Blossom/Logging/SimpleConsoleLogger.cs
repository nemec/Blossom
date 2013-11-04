using System;
using Blossom.Exceptions;

namespace Blossom.Logging
{
    /// <summary>
    /// Simple threadsafe logger implementation that
    /// prints to the console.
    /// </summary>
    public class SimpleConsoleLogger : ILogger
    {
        private readonly object _lock;

        /// <inheritdoc />
        public LogLevel DisplayLogLevel { get; set; }

        /// <inheritdoc />
        public LogLevel AbortLogLevel { get; set; }

        /// <summary>
        /// Separator between title and message in logs.
        /// </summary>
        protected string TitleSeparator = ": ";

        /// <summary>
        /// Default generic log title.
        /// </summary>
        protected string LogTitle = "Log";

        /// <summary>
        /// Defauld debugging title.
        /// </summary>
        protected string DebugTitle = "Debug";

        /// <summary>
        /// Default verbose title.
        /// </summary>
        protected string VerboseTitle = "Verbose";

        /// <summary>
        /// Default info title.
        /// </summary>
        protected string InfoTitle = "Info";

        /// <summary>
        /// Default warning title.
        /// </summary>
        protected string WarnTitle = "Warn";

        /// <summary>
        /// Default error title.
        /// </summary>
        protected string ErrorTitle = "Error";

        /// <summary>
        /// Default fatal title.
        /// </summary>
        protected string FatalTitle = "Fatal";

        /// <summary>
        /// Default abort title.
        /// </summary>
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

        /// <inheritdoc />
        public virtual void Tick(string message)
        {
            message = message.Length > Console.WindowWidth - 1 ?
                message.Substring(0, Math.Min(message.Length, Console.WindowWidth - 1)) :
                message.PadRight(Console.WindowWidth - 1);

            lock (_lock)
            {
                var currentCol = Console.CursorLeft;
                var currentLine = Console.CursorTop;
                Console.SetCursorPosition(0, Console.WindowTop + Console.WindowHeight - 1);
                Console.Write(message);
                Console.SetCursorPosition(currentCol, currentLine);
            }
        }

        /// <inheritdoc />
        public virtual void ClearTicker()
        {
            lock (_lock)
            {
                var currentCol = Console.CursorLeft;
                var currentLine = Console.CursorTop;
                Console.SetCursorPosition(0, Console.WindowTop + Console.WindowHeight - 1);
                Console.Write("".PadRight(Console.WindowWidth - 1));
                Console.SetCursorPosition(currentCol, currentLine);
            }
        }

        /// <inheritdoc />
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

        /// <inheritdoc />
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

        /// <inheritdoc />
        public void Log(LogLevel level, string message, Exception exception)
        {
            Log(level, LogTitle, message, exception);
        }

        /// <inheritdoc />
        public void Debug(string message, Exception exception)
        {
            Log(LogLevel.Debug, DebugTitle, message, exception);
        }

        /// <inheritdoc />
        public void Verbose(string message, Exception exception)
        {
            Log(LogLevel.Verbose, VerboseTitle, message, exception);
        }

        /// <inheritdoc />
        public void Info(string message, Exception exception)
        {
            Log(LogLevel.Info, InfoTitle, message, exception);
        }

        /// <inheritdoc />
        public void Warn(string message, Exception exception)
        {
            Log(LogLevel.Warn, WarnTitle, message, exception);
        }

        /// <inheritdoc />
        public void Error(string message, Exception exception)
        {
            Log(LogLevel.Error, ErrorTitle, message, exception);
        }

        /// <inheritdoc />
        public void Fatal(string message, Exception exception)
        {
            Log(LogLevel.Fatal, FatalTitle, message, exception);
        }

        #endregion
    }
}