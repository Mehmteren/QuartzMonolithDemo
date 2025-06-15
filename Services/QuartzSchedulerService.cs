using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Quartz.Impl;
using Quartz.Spi;
using Quartz;
using QuartzMonolithDemo.Jobs;
using System.Collections.Specialized;
using System.Threading.Tasks;
using System.Threading;
using System;
using QuartzMonolithDemo.Services;

public class QuartzSchedulerService : IHostedService
{
    private IScheduler _scheduler;
    private readonly IServiceProvider _provider;
    private readonly ILogger<QuartzSchedulerService> _logger;

    public QuartzSchedulerService(IServiceProvider provider, ILogger<QuartzSchedulerService> logger)
    {
        _provider = provider;
        _logger = logger;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        var factory = new StdSchedulerFactory(new NameValueCollection
        {
            ["quartz.scheduler.instanceId"] = Environment.MachineName
        });

        _scheduler = await factory.GetScheduler();
        _scheduler.JobFactory = new MicrosoftDependencyInjectionJobFactory(_provider);
        await _scheduler.Start();

        var job = JobBuilder.Create<LogJob>().WithIdentity("logJob", "group1").Build();
        var trigger = TriggerBuilder.Create().StartNow().WithSimpleSchedule(x => x.WithIntervalInSeconds(10).RepeatForever()).Build();

        await _scheduler.ScheduleJob(job, trigger);
        _logger.LogInformation("Quartz Scheduler started");
    }

    public Task StopAsync(CancellationToken cancellationToken) => _scheduler?.Shutdown() ?? Task.CompletedTask;
}


