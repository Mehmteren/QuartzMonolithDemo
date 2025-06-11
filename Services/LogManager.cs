using QuartzMonolithDemo.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using QuartzMonolithDemo.Services.Interface;

namespace QuartzMonolithDemo.Services
{
    public class LogManager : ILogManager
    {
        private readonly string _logDirectory = "logs";
        private readonly string _instanceName;
        private readonly object _lockObject = new object();
        private readonly ILogger<LogManager> _logger;

        public LogManager(ILogger<LogManager> logger)
        {
            _logger = logger;
            _instanceName = Environment.MachineName;
            Directory.CreateDirectory(_logDirectory);
        }

        public void WriteLog(LogEntry entry)
        {
            try
            {
                string filePath = GetLogFilePath();
                string logLine = $"[{entry.Timestamp:yyyy-MM-dd HH:mm:ss}] [{entry.InstanceName}] {entry.Message}";

                lock (_lockObject)
                {
                    File.AppendAllText(filePath, logLine + Environment.NewLine);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to write log entry");
            }
        }

        public List<string> ReadLastLogs(int count = 10)
        {
            try
            {
                string filePath = GetLogFilePath();
                if (!File.Exists(filePath))
                    return new List<string> { $"No log file found for instance {_instanceName}" };

                var lines = File.ReadAllLines(filePath);
                return lines.Reverse().Take(count).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to read logs");
                return new List<string> { $"Error reading logs: {ex.Message}" };
            }
        }

        private string GetLogFilePath()
        {
            return Path.Combine(_logDirectory, $"log-{_instanceName}.txt");
        }
    }
}