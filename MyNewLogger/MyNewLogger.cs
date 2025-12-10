using System;
using System.IO;

namespace MyNewLogger
{
    public interface ILogger
    {
        void LogInformation(string message);
        void LogError(Exception exception, string? additionalMessage = null);
    }
    public class ConsoleLogger : ILogger
    {
        public void LogInformation(string message)
        {
            Console.WriteLine(message);
        }
        public void LogError(Exception exception, string? additionalMessage = null)
        {
            Console.WriteLine(exception.Message);
            if (additionalMessage != null) Console.WriteLine(additionalMessage);
        }
    }
    public class FileLogger : ILogger
    {
        public void LogInformation(string message)
        {
            using (StreamWriter writer = new StreamWriter("log.txt", true))
            {
                writer.WriteLine(message);
            }
        }
        public void LogError(Exception exception, string? additionalMessage = null)
        {
            using (StreamWriter writer = new StreamWriter("log.txt", true)) 
            {
                writer.WriteLine(exception.Message);
                if (additionalMessage != null) writer.WriteLine(additionalMessage);
            }
        }
    }

    public class CompositeLogger : ILogger
    {
        List<ILogger> _loggers;
        public ILogger Logger {  get; set; }
        public CompositeLogger (params ILogger[] loggers)
        {
            _loggers = new List<ILogger> (loggers);
        }
        public void LogInformation(string message)
        {
            foreach (ILogger logger in _loggers) 
                logger.LogInformation(message);
        }
        public void LogError(Exception exception, string? additionalMessage = null)
        {
            foreach (ILogger logger in _loggers)
                logger.LogError(exception, additionalMessage);
        }
    }
}
