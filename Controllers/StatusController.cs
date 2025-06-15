using Microsoft.AspNetCore.Mvc;
using QuartzMonolithDemo.Services.Interface;
using System;

[ApiController]
[Route("[controller]")]
public class StatusController : ControllerBase
{
    private readonly IJobMonitor _monitor;
    public StatusController(IJobMonitor monitor) => _monitor = monitor;

    [HttpGet("health")]
    public IActionResult Health()
    {
        var status = _monitor.GetJobStatusWithHealth("LogJob", 10);
        return Ok(new
        {
            Status = status.HealthStatus,
            Instance = Environment.MachineName,
            ProcessId = Environment.ProcessId,
            Job = status,
            Timestamp = DateTime.UtcNow
        });
    }
}