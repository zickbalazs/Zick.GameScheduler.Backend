using Docker.DotNet;
using Microsoft.Extensions.Logging;
using Quartz;
using Zick.GameScheduler.Backend.Data;
using Zick.GameScheduler.Backend.Data.Models;

namespace Zick.GameScheduler.Backend.Scheduler.Jobs;

public class StartSessionJob(ILogger<StartSessionJob> logger, 
    ApplicationContext<RacingUserIdentity> ctx, 
    //IDockerClient dockerClient,
    ISchedulerFactory schedFactory) : IJob
{
    public async Task Execute(IJobExecutionContext context)
    {
        string sessionId = context.JobDetail.Key.Name;
        logger.LogInformation("Locking registrations and starting server for session: {sessionId}", sessionId);

        try
        {
            logger.LogTrace("updating session status to starting for {session}", sessionId);
            var session = ctx.Sessions.First(x => x.Id == new Guid(sessionId));
            session.Status = SessionStatus.Starting;
            await ctx.SaveChangesAsync();
        }
        catch (Exception e)
        {
            if (e is InvalidOperationException || e is ArgumentNullException)
                logger.LogError("session doesn't exist for id {id}, terminating job", sessionId);
            else
            {
                logger.LogError("a database error has occured, trying again in five minutes...");

                var job = JobBuilder
                    .Create<StartSessionJob>()
                    .WithIdentity($"{sessionId}", "session-wait-jobs")
                    .Build();
                
                var trigger = TriggerBuilder
                    .Create()
                    .ForJob(job)
                    .StartAt(DateTime.Now + TimeSpan.FromMinutes(5))
                    .Build();

                await (await schedFactory.GetScheduler()).ScheduleJob(job, trigger);
            }
        }
        
        logger.LogInformation("Starting container for session: {id}", sessionId);
        var container = "asdasdasdasd";

        var watchJob = JobBuilder
            .Create<WatchSessionJob>()
            .WithIdentity(sessionId, "watch-session")
            .WithDescription($"containerId:{container}")
            .Build();

        var watchTrigger = TriggerBuilder
            .Create()
            .StartNow()
            .WithSimpleSchedule(x =>
            {
                x.WithIntervalInSeconds(30);
                x.RepeatForever();
            })
            .ForJob(watchJob)
            .Build();

        await (await schedFactory.GetScheduler()).ScheduleJob(watchJob, watchTrigger);
    }
}