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

        public IDeploymentContext Context { get; set; }

        public LogLevel DisplayLogLevel { get; set; }

        public LogLevel AbortLogLevel { get; set; }

        public SimpleConsoleLogger()
        {
            _lock = new object();
            AbortLogLevel = LogLevel.Fatal;
        }

        private bool CheckLevel(LogLevel level)
        {
            if(AbortLogLevel <= level)
            {
                throw new AbortExecutionException();
            }
            return DisplayLogLevel <= level;
        }

        private static void PrintException(Exception exception)
        {
            if (exception != null)
            {
                Console.Error.WriteLine("\t" + exception.Message);
            }
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

        public void Debug(string message, Exception exception)
        {
            lock (_lock)
            {
                if (!CheckLevel(LogLevel.Debug)) return;

                Console.WriteLine("Debug: " + message);
                PrintException(exception);
            }
        }

        public void Verbose(string message, Exception exception)
        {
            lock (_lock)
            {
                if (!CheckLevel(LogLevel.Verbose)) return;

                Console.WriteLine("Verbose: " + message);
                PrintException(exception);
            }
        }

        public void Info(string message, Exception exception)
        {
            lock (_lock)
            {
                if (!CheckLevel(LogLevel.Info)) return;

                Console.WriteLine("Info: " + message);
                PrintException(exception);
            }
        }

        public void Warn(string message, Exception exception)
        {
            lock (_lock)
            {
                if (!CheckLevel(LogLevel.Warn)) return;

                Console.WriteLine("Warn: " + message);
                PrintException(exception);
            }
        }
        public void Error(string message, Exception exception)
        {
            lock (_lock)
            {
                if (!CheckLevel(LogLevel.Error)) return;

                Console.WriteLine("Error: " + message);
                PrintException(exception);
            }
        }

        public void Fatal(string message, Exception exception)
        {
            lock (_lock)
            {
                if (!CheckLevel(LogLevel.Fatal)) return;

                Console.WriteLine("Fatal: " + message);
                PrintException(exception);
            }
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