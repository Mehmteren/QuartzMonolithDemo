using Microsoft.AspNetCore.Mvc;
using System;
using QuartzMonolithDemo.Services.Interface;

namespace QuartzMonolithDemo.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LogsController : ControllerBase
    {
        private readonly ILogManager _logManager;

        public LogsController(ILogManager logManager)
        {
            _logManager = logManager;
        }

        [HttpGet]
        public IActionResult GetLogs([FromQuery] int count = 10)
        {
            var logs = _logManager.ReadLastLogs(count);

            return Ok(new
            {
                Instance = Environment.MachineName,
                Data = logs
            });
        }
    }
}