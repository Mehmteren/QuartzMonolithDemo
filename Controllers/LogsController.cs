using Microsoft.AspNetCore.Mvc;
using QuartzMonolithDemo.Services.Interface;
using System;

[ApiController]
[Route("api/[controller]")]
public class LogsController : ControllerBase
{
    private readonly ILogManager _logManager;
    public LogsController(ILogManager logManager) => _logManager = logManager;

    [HttpGet]
    public IActionResult GetLogs([FromQuery] int count = 10) => Ok(new
    {
        Instance = Environment.MachineName,
        Data = _logManager.ReadLastLogs(count)
    });
}