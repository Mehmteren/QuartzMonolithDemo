using Quartz.Spi;
using Quartz;
using System;

namespace QuartzMonolithDemo.Services
{
    public class MicrosoftDependencyInjectionJobFactory : IJobFactory
    {
        private readonly IServiceProvider _provider;
        public MicrosoftDependencyInjectionJobFactory(IServiceProvider provider) => _provider = provider;
        public IJob NewJob(TriggerFiredBundle bundle, IScheduler scheduler) => _provider.GetService(bundle.JobDetail.JobType) as IJob;
        public void ReturnJob(IJob job) { }
    }
}
