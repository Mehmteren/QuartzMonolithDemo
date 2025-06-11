using System;

namespace QuartzMonolithDemo.Models
{
    public class JobHealthInfo
    {
        public string JobName { get; set; }
        public string HealthStatus { get; set; }
        public bool IsHealthy { get; set; }
        public string Message { get; set; }
        public int? SecondsSinceLastRun { get; set; }
        public DateTime? LastExecutionTime { get; set; }
        public bool? LastSuccess { get; set; }
        public long? ExecutionCount { get; set; }
        public string InstanceName { get; set; }
        public string LastError { get; set; }
    }
}