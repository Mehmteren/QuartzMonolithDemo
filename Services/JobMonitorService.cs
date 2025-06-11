using System;
using System.Collections.Concurrent;
using QuartzMonolithDemo.Models;
using QuartzMonolithDemo.Services.Interface;
using Microsoft.Extensions.Logging;

namespace QuartzMonolithDemo.Services
{
    public class JobMonitorService : IJobMonitor
    {
        private readonly ConcurrentDictionary<string, JobStatus> _jobStatuses = new();
        private readonly ILogger<JobMonitorService> _logger;

        public JobMonitorService(ILogger<JobMonitorService> logger)
        {
            _logger = logger;
        }

        public void UpdateJobStatus(string jobName, bool isSuccess, string message = null, Exception exception = null)
        {
            try
            {
                var currentStatus = GetJobStatus(jobName);
                var status = new JobStatus
                {
                    JobName = jobName,
                    LastExecutionTime = DateTime.UtcNow,
                    IsSuccess = isSuccess,
                    Message = message ?? (isSuccess ? "Executed successfully" : "Execution failed"),
                    Exception = exception?.Message,
                    InstanceName = Environment.MachineName,
                    ExecutionCount = (currentStatus?.ExecutionCount ?? 0) + 1
                };

                _jobStatuses.AddOrUpdate(jobName, status, (key, oldValue) => status);
                _logger.LogDebug("Job status updated for {JobName}: {IsSuccess}", jobName, isSuccess);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to update job status for {JobName}", jobName);
            }
        }

        public JobHealthInfo GetJobHealth(string jobName, int expectedIntervalSeconds = 10)
        {
            var status = GetJobStatus(jobName);

            if (status == null)
            {
                return new JobHealthInfo
                {
                    JobName = jobName,
                    HealthStatus = "Unknown",
                    IsHealthy = false,
                    Message = "No execution records found",
                    SecondsSinceLastRun = null
                };
            }

            var secondsSinceLastRun = (int)(DateTime.UtcNow - status.LastExecutionTime).TotalSeconds;
            var toleranceSeconds = expectedIntervalSeconds + 5;

            string healthStatus;
            bool isHealthy;
            string message;

            if (secondsSinceLastRun <= toleranceSeconds)
            {
                healthStatus = "Healthy";
                isHealthy = true;
                message = $"Running normally (last run: {secondsSinceLastRun}s ago)";
            }
            else if (secondsSinceLastRun <= toleranceSeconds * 2)
            {
                healthStatus = "Warning";
                isHealthy = false;
                message = $"Delayed execution (last run: {secondsSinceLastRun}s ago)";
            }
            else
            {
                healthStatus = "Critical";
                isHealthy = false;
                message = $"Job appears stopped (last run: {secondsSinceLastRun}s ago)";
            }

            return new JobHealthInfo
            {
                JobName = jobName,
                HealthStatus = healthStatus,
                IsHealthy = isHealthy,
                Message = message,
                SecondsSinceLastRun = secondsSinceLastRun,
                LastExecutionTime = status.LastExecutionTime,
                LastSuccess = status.IsSuccess,
                ExecutionCount = status.ExecutionCount,
                InstanceName = status.InstanceName,
                LastError = status.Exception
            };
        }

        private JobStatus GetJobStatus(string jobName)
        {
            _jobStatuses.TryGetValue(jobName, out var status);
            return status;
        }
    }
}
