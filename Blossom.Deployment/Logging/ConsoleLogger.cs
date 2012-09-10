using System;

namespace Blossom.Deployment.Logging
{
    public class ConsoleLogger : ILogger
    {
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

        public void Debug(string message)
        {
            Console.WriteLine("Debug: " + message);
        }

        public void Info(string message)
        {
            Console.WriteLine("Info: " + message);
        }

        public void Warn(string message)
        {
            Console.WriteLine("Warn: " + message);
        }

        public void Warn(string message, Exception exception)
        {
            Console.WriteLine("Warn: " + message);
            Console.WriteLine("\t" + exception.Message);
        }

        public void Error(string message)
        {
            Console.WriteLine("Error: " + message);
        }

        public void Error(string message, Exception exception)
        {
            Console.WriteLine("Error: " + message);
            Console.WriteLine("\t" + exception.Message);
        }

        public void Fatal(string message)
        {
            Console.WriteLine("Fatal: " + message);
        }

        public void Fatal(string message, Exception exception)
        {
            Console.WriteLine("Fatal: " + message);
            Console.WriteLine("\t" + exception.Message);
        }
    }
}