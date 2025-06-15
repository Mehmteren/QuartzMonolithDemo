using Microsoft.Extensions.Logging;
using QuartzMonolithDemo.Services.Interface;
using System.Collections.Concurrent;
using System;

public class JobMonitorService : IJobMonitor
{
    private readonly ConcurrentDictionary<string, JobStatus> _jobStatuses = new();
    private readonly ILogger<JobMonitorService> _logger;

    public JobMonitorService(ILogger<JobMonitorService> logger) => _logger = logger;

    public void UpdateJobStatus(string jobName, bool isSuccess, string message = null, Exception exception = null)
    {
        var now = DateTime.UtcNow;
        var status = new JobStatus
        {
            JobName = jobName,
            LastExecutionTime = now,
            IsSuccess = isSuccess,
            Message = message ?? (isSuccess ? "Success" : "Failure"),
            Exception = exception?.Message,
            InstanceName = Environment.MachineName,
            ExecutionCount = (_jobStatuses.TryGetValue(jobName, out var old) ? old.ExecutionCount : 0) + 1
        };
        _jobStatuses.AddOrUpdate(jobName, status, (k, v) => status);
    }

    public JobStatus GetJobStatusWithHealth(string jobName, int expectedIntervalSeconds = 10)
    {
        if (!_jobStatuses.TryGetValue(jobName, out var status))
        {
            return new JobStatus
            {
                JobName = jobName,
                IsSuccess = false,
                Message = "No record found",
                HealthStatus = "Unknown",
                IsHealthy = false
            };
        }

        var seconds = (int)(DateTime.UtcNow - status.LastExecutionTime).TotalSeconds;
        status.SecondsSinceLastRun = seconds;
        if (seconds <= expectedIntervalSeconds + 5)
        {
            status.HealthStatus = "Healthy";
            status.IsHealthy = true;
        }
        else if (seconds <= (expectedIntervalSeconds + 5) * 2)
        {
            status.HealthStatus = "Warning";
            status.IsHealthy = false;
        }
        else
        {
            status.HealthStatus = "Critical";
            status.IsHealthy = false;
        }

        return status;
    }
}