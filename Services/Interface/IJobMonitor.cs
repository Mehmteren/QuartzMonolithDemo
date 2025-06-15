using System;

namespace QuartzMonolithDemo.Services.Interface
{
    public interface IJobMonitor
    {
        void UpdateJobStatus(string jobName, bool isSuccess, string message = null, Exception exception = null);
        JobStatus GetJobStatusWithHealth(string jobName, int expectedIntervalSeconds = 10);
    }
}