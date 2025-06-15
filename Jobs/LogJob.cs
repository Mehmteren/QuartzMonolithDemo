using Quartz;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using System;
using QuartzMonolithDemo.Services.Interface;

namespace QuartzMonolithDemo.Jobs
{
    public class LogJob : IJob
    {
        private readonly ILogManager _logManager;
        private readonly IJobMonitor _jobMonitor;
        private readonly ILogger<LogJob> _logger;
        private const string JOB_NAME = "LogJob";

        public LogJob(ILogManager logManager, IJobMonitor jobMonitor, ILogger<LogJob> logger)
        {
            _logManager = logManager;
            _jobMonitor = jobMonitor;
            _logger = logger;
        }

        public Task Execute(IJobExecutionContext context)
        {
            try
            {
                string message = "LogJob executed successfully";
                _logManager.WriteLog(message);
                _logger.LogInformation("{Message} on {Instance}", message, Environment.MachineName);

                _jobMonitor.UpdateJobStatus(JOB_NAME, true, message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "LogJob execution failed on {Instance}", Environment.MachineName);
                _jobMonitor.UpdateJobStatus(JOB_NAME, false, "LogJob execution failed", ex);
            }

            return Task.CompletedTask;
        }
    }
}