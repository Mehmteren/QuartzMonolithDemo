using Microsoft.Extensions.Logging;
using QuartzMonolithDemo.Services.Interface;
using System.Collections.Generic;
using System.IO;
using System;
using System.Linq;

namespace QuartzMonolithDemo.Services
{
    public class LogManager : ILogManager
    {
        private readonly string _logDir = "logs";
        private readonly string _instance = Environment.MachineName;
        private readonly object _lock = new();
        private readonly ILogger<LogManager> _logger;

        public LogManager(ILogger<LogManager> logger)
        {
            _logger = logger;
            Directory.CreateDirectory(_logDir);
        }

        public void WriteLog(string message)
        {
            try
            {
                var line = $"[{DateTime.UtcNow:yyyy-MM-dd HH:mm:ss}] [{_instance}] {message}";
                lock (_lock)
                {
                    File.AppendAllText(Path.Combine(_logDir, $"log-{_instance}.txt"), line + Environment.NewLine);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Logging failed");
            }
        }

        public List<string> ReadLastLogs(int count = 10)
        {
            var file = Path.Combine(_logDir, $"log-{_instance}.txt");

            if (!File.Exists(file))
                return new List<string> { "No logs found." };

            return File.ReadAllLines(file)
                       .Reverse()
                       .Take(count)
                       .ToList();
        }
    }
}
