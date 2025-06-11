using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Quartz;
using Quartz.Impl;
using System.Threading;
using System.Threading.Tasks;
using QuartzMonolithDemo.Jobs;
using System;
using System.Collections.Specialized;
using Quartz.Spi;

namespace QuartzMonolithDemo.Services
{
    public class QuartzSchedulerService : IHostedService
    {
        private IScheduler _scheduler;
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<QuartzSchedulerService> _logger;

        public QuartzSchedulerService(IServiceProvider serviceProvider, ILogger<QuartzSchedulerService> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Starting Quartz Scheduler on instance {Instance}...", Environment.MachineName);

                var properties = new NameValueCollection
                {
                    ["quartz.scheduler.instanceName"] = "QuartzMonolithDemo",
                    ["quartz.scheduler.instanceId"] = Environment.MachineName,
                    ["quartz.threadPool.type"] = "Quartz.Simpl.SimpleThreadPool, Quartz",
                    ["quartz.threadPool.threadCount"] = "5",
                    ["quartz.jobStore.type"] = "Quartz.Simpl.RAMJobStore, Quartz"
                };

                var factory = new StdSchedulerFactory(properties);
                _scheduler = await factory.GetScheduler(cancellationToken);
                _scheduler.JobFactory = new MicrosoftDependencyInjectionJobFactory(_serviceProvider);

                await _scheduler.Start(cancellationToken);

                var job = JobBuilder.Create<LogJob>()
                    .WithIdentity("logJob", "group1")
                    .Build();

                var trigger = TriggerBuilder.Create()
                    .WithIdentity("logTrigger", "group1")
                    .StartNow()
                    .WithSimpleSchedule(x => x
                        .WithIntervalInSeconds(10)
                        .RepeatForever())
                    .Build();

                await _scheduler.ScheduleJob(job, trigger, cancellationToken);

                _logger.LogInformation("Quartz Scheduler started successfully with LogJob scheduled every 10 seconds on {Instance}", Environment.MachineName);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to start Quartz Scheduler on {Instance}", Environment.MachineName);
                throw;
            }
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            try
            {
                if (_scheduler != null)
                {
                    _logger.LogInformation("Shutting down Quartz Scheduler on {Instance}...", Environment.MachineName);
                    await _scheduler.Shutdown(cancellationToken);
                    _logger.LogInformation("Quartz Scheduler shut down successfully on {Instance}", Environment.MachineName);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while shutting down Quartz Scheduler on {Instance}", Environment.MachineName);
            }
        }
    }

    public class MicrosoftDependencyInjectionJobFactory : IJobFactory
    {
        private readonly IServiceProvider _serviceProvider;

        public MicrosoftDependencyInjectionJobFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public IJob NewJob(TriggerFiredBundle bundle, IScheduler scheduler)
        {
            try
            {
                var job = _serviceProvider.GetService(bundle.JobDetail.JobType) as IJob;
                return job ?? (IJob)Activator.CreateInstance(bundle.JobDetail.JobType);
            }
            catch
            {
                return (IJob)Activator.CreateInstance(bundle.JobDetail.JobType);
            }
        }

        public void ReturnJob(IJob job)
        {
        }
    }
}