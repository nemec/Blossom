using System;

namespace Blossom.Deployment.Logging
{
    /// <summary>
    /// Simple threadsafe logger implementation that
    /// prints to the console.
    /// </summary>
    public class SimpleConsoleLogger : ILogger
    {
        private object _lock;

        public SimpleConsoleLogger()
        {
            _lock = new object();
        }

        private void PrintException(Exception exception)
        {
            if (exception != null)
            {
                lock (_lock)
                {
                    Console.Error.WriteLine("\t" + exception.Message);
                }
            }
        }

        public void Tick(string message)
        {
            int currentCol = Console.CursorLeft;
            int currentLine = Console.CursorTop;

            if (message.Length > Console.WindowWidth - 1)
            {
                message = message.Substring(0, Math.Min(message.Length, Console.WindowWidth - 1));
            }
            else
            {
                message = message.PadRight(Console.WindowWidth - 1);
            }

            lock (_lock)
            {
                Console.SetCursorPosition(0, Console.WindowTop + Console.WindowHeight - 1);
                Console.Write(message);
                Console.SetCursorPosition(currentCol, currentLine);
            }
        }

        public void ClearTicker()
        {
            int currentCol = Console.CursorLeft;
            int currentLine = Console.CursorTop;

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
                Console.WriteLine("Debug: " + message);
                PrintException(exception);
            }
        }

        public void Verbose(string message, Exception exception)
        {
            lock (_lock)
            {
                Info(message, exception);
                PrintException(exception);
            }
        }

        public void Info(string message, Exception exception)
        {
            lock (_lock)
            {
                Console.WriteLine("Info: " + message);
                PrintException(exception);
            }
        }

        public void Warn(string message, Exception exception)
        {
            lock (_lock)
            {
                Console.WriteLine("Warn: " + message);
                PrintException(exception);
            }
        }
        public void Error(string message, Exception exception)
        {
            lock (_lock)
            {
                Console.WriteLine("Error: " + message);
                PrintException(exception);
            }
        }

        public void Fatal(string message, Exception exception)
        {
            lock (_lock)
            {
                Console.WriteLine("Fatal: " + message);
                PrintException(exception);
            }
        }
    }
}