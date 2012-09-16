using System;

namespace Blossom.Deployment.Logging
{
    public class SimpleConsoleLogger : ILogger
    {
        private void PrintException(Exception exception)
        {
            if (exception != null)
            {
                Console.Error.WriteLine("\t" + exception.Message);
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

            Console.SetCursorPosition(0, Console.WindowTop + Console.WindowHeight - 1);
            Console.Write(message);
            Console.SetCursorPosition(currentCol, currentLine);
        }

        public void ClearTicker()
        {
            int currentCol = Console.CursorLeft;
            int currentLine = Console.CursorTop;
            Console.SetCursorPosition(0, Console.WindowTop + Console.WindowHeight - 1);
            Console.Write("".PadRight(Console.WindowWidth - 1));
            Console.SetCursorPosition(currentCol, currentLine);
        }

        public void Debug(string message, Exception exception)
        {
            Console.WriteLine("Debug: " + message);
            PrintException(exception);
        }

        public void Verbose(string message, Exception exception)
        {
            Info(message, exception);
            PrintException(exception);
        }

        public void Info(string message, Exception exception)
        {
            Console.WriteLine("Info: " + message);
            PrintException(exception);
        }

        public void Warn(string message, Exception exception)
        {
            Console.WriteLine("Warn: " + message);
            PrintException(exception);
        }
        public void Error(string message, Exception exception)
        {
            Console.WriteLine("Error: " + message);
            PrintException(exception);
        }

        public void Fatal(string message, Exception exception)
        {
            Console.WriteLine("Fatal: " + message);
            PrintException(exception);
        }
    }
}