using Microsoft.AspNetCore.Mvc;
using System;
using QuartzMonolithDemo.Services.Interface;

namespace QuartzMonolithDemo.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class StatusController : ControllerBase
    {
        private readonly IJobMonitor _jobMonitor;

        public StatusController(IJobMonitor jobMonitor)
        {
            _jobMonitor = jobMonitor;
        }

        [HttpGet("health")]
        public IActionResult Health()
        {
            var jobHealth = _jobMonitor.GetJobHealth("LogJob", 10);

            return Ok(new
            {
                Status = jobHealth.IsHealthy ? "Healthy" : "Unhealthy",
                Instance = Environment.MachineName,
                ProcessId = Environment.ProcessId,
                Job = new
                {
                    Name = jobHealth.JobName,
                    Status = jobHealth.HealthStatus,
                    IsHealthy = jobHealth.IsHealthy,
                    Message = jobHealth.Message,
                    LastRun = jobHealth.LastExecutionTime,
                    SecondsSinceLastRun = jobHealth.SecondsSinceLastRun,
                    ExecutionCount = jobHealth.ExecutionCount,
                    LastSuccess = jobHealth.LastSuccess
                },
                Timestamp = DateTime.UtcNow
            });
        }
    }
}