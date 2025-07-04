using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace XWopcUA.Utils
{
    public enum LogLevel
    {
        Debug,
        Info,
        Warning,
        Error,
        Fatal
    }

    public class Logger
    {
        private static Logger _instance;
        private readonly object _lockObject = new object();
        private readonly string _logFilePath;
        private readonly Queue<string> _logBuffer = new Queue<string>();
        private readonly int _maxBufferSize = 1000;

        public event EventHandler<LogEventArgs> LogMessageReceived;

        private Logger()
        {
            string logDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Logs");
            if (!Directory.Exists(logDir))
            {
                Directory.CreateDirectory(logDir);
            }
            _logFilePath = Path.Combine(logDir, $"XWopcUA_{DateTime.Now:yyyyMMdd}.log");
        }

        public static Logger Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new Logger();
                }
                return _instance;
            }
        }

        public void Log(LogLevel level, string message, Exception ex = null)
        {
            string timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
            string logEntry = $"[{timestamp}] [{level}] {message}";
            
            if (ex != null)
            {
                logEntry += $"\r\nException: {ex.Message}\r\nStackTrace: {ex.StackTrace}";
            }

            lock (_lockObject)
            {
                _logBuffer.Enqueue(logEntry);
                if (_logBuffer.Count > _maxBufferSize)
                {
                    _logBuffer.Dequeue();
                }

                try
                {
                    File.AppendAllText(_logFilePath, logEntry + Environment.NewLine);
                }
                catch
                {
                }
            }

            LogMessageReceived?.Invoke(this, new LogEventArgs { Level = level, Message = message, Timestamp = DateTime.Now });
        }

        public void Debug(string message) => Log(LogLevel.Debug, message);
        public void Info(string message) => Log(LogLevel.Info, message);
        public void Warning(string message) => Log(LogLevel.Warning, message);
        public void Error(string message, Exception ex = null) => Log(LogLevel.Error, message, ex);
        public void Fatal(string message, Exception ex = null) => Log(LogLevel.Fatal, message, ex);

        public List<string> GetRecentLogs(int count = 100)
        {
            lock (_lockObject)
            {
                List<string> logs = new List<string>(_logBuffer);
                if (logs.Count > count)
                {
                    return logs.GetRange(logs.Count - count, count);
                }
                return logs;
            }
        }
    }

    public class LogEventArgs : EventArgs
    {
        public LogLevel Level { get; set; }
        public string Message { get; set; }
        public DateTime Timestamp { get; set; }
    }
}