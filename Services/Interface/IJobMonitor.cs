using System;
using QuartzMonolithDemo.Models;

namespace QuartzMonolithDemo.Services.Interface
{
    public interface IJobMonitor
    {
        void UpdateJobStatus(string jobName, bool isSuccess, string message = null, Exception exception = null);
        JobHealthInfo GetJobHealth(string jobName, int expectedIntervalSeconds = 10);
    }
}