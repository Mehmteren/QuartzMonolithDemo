using System;

public class JobStatus
{
    public string JobName { get; set; }
    public DateTime LastExecutionTime { get; set; }
    public bool IsSuccess { get; set; }
    public string Message { get; set; }
    public string Exception { get; set; }
    public string InstanceName { get; set; }
    public long ExecutionCount { get; set; }
    public int? SecondsSinceLastRun { get; set; }
    public string HealthStatus { get; set; }
    public bool IsHealthy { get; set; }
}