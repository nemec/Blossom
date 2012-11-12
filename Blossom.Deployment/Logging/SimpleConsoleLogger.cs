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

        public SimpleConsoleLogger()
        {
            _lock = new object();
            AbortLogLevel = LogLevel.Fatal;
        }

        public void Tick(string message)
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

        public void ClearTicker()
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

        public void Log(LogLevel level, string message, Exception exception)
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

                Console.WriteLine(message);
                if (exception != null)
                {
                    Console.Error.WriteLine("\t" + exception.Message);
                }
            }
        }

        public void Debug(string message, Exception exception)
        {
            Log(LogLevel.Debug, "Debug: " + message, exception);
        }

        public void Verbose(string message, Exception exception)
        {
            Log(LogLevel.Verbose, "Verbose: " + message, exception);
        }

        public void Info(string message, Exception exception)
        {
            Log(LogLevel.Info, "Info: " + message, exception);
        }

        public void Warn(string message, Exception exception)
        {
            Log(LogLevel.Warn, "Warn: " + message, exception);
        }
        public void Error(string message, Exception exception)
        {
            Log(LogLevel.Error, "Error: " + message, exception);
        }

        public void Fatal(string message, Exception exception)
        {
            Log(LogLevel.Fatal, "Fatal: " + message, exception);
        }

        public void Abort(string message, Exception exception)
        {
            lock(_lock)
            {
                Console.Error.WriteLine("Abort: {0}", message);
                if(exception is AbortExecutionException)
                {
                    throw exception;
                }
                throw new AbortExecutionException(message, exception);
            }
        }
    }
}