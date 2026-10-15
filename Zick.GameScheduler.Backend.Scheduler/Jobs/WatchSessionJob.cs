using Docker.DotNet;
using Microsoft.Extensions.Logging;
using Quartz;
using Zick.GameScheduler.Backend.Scheduler.Services;

namespace Zick.GameScheduler.Backend.Scheduler.Jobs;

public class WatchSessionJob(IDockerClient dockerClient,
    ISchedulerFactory schedulerFactory,
    ILogger<WatchSessionJob> logger) : IJob
{
    public async Task Execute(IJobExecutionContext context)
    {
        var scheduler = await schedulerFactory.GetScheduler();
        var sessionId = context.JobDetail.Key.Name;

        logger.LogInformation("watching session: {sessionId}", sessionId);
        
        
        var job =JobBuilder.Create<EndSessionJob>()
            .WithIdentity(sessionId, "end-jobs")
            .WithDescription(context.JobDetail.Description)
            .Build();

        var trigger = TriggerBuilder
            .Create()
            .ForJob(job)
            .StartAt(DateTime.Now + TimeSpan.FromMinutes(5))
            .Build();

        await scheduler.ScheduleJob(job, trigger);
    }
}