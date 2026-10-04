using Docker.DotNet;
using Microsoft.Extensions.Logging;
using Quartz;
using Zick.GameScheduler.Backend.Data;
using Zick.GameScheduler.Backend.Data.Models;

namespace Zick.GameScheduler.Backend.Scheduler.Jobs;

public class StartSessionJob(ILogger<StartSessionJob> logger, 
    ApplicationContext<RacingUserIdentity> ctx, 
    IDockerClient dockerClient,
    ISchedulerFactory schedFactory) : IJob
{
    public async Task Execute(IJobExecutionContext context)
    {
        string sessionId = context.JobDetail.Key.Name;
        logger.LogInformation("[session: {sessionId}]: Locking registrations and starting job", sessionId);
        
        try
        {
            logger.LogTrace("[session: {sessionId}]: Entering db for status update", sessionId);
            await UpdateSessionToStatus(sessionId, SessionStatus.Starting);
        }
        catch (Exception e)
        {
            logger.LogTrace("[session: {sessionId}]: Entering db failure for status update", sessionId);
            await RestartSessionJob(e, sessionId);
            return;
        }
        
        string containerId = await StartRaceContainer(sessionId);

        if (containerId == string.Empty)
        {
            logger.LogTrace("[session: {sessionId}]: Container start failed", sessionId);
            await UpdateSessionToStatus(sessionId, SessionStatus.Error);
            return;
        }
        logger.LogInformation("[session: {sessionId}]: Container start succeeded", sessionId);

        await CreateWatchJob(sessionId, containerId);
    }

    private async Task RestartSessionJob(Exception e, string sessionId)
    {
        if (e is InvalidOperationException || e is ArgumentNullException)
            logger.LogError("[session: {sessionId}] session doesn't exist, terminating job", sessionId);
        else
        {
            logger.LogError("[session: {sessionId}]: a database error has occured, delaying start by five minutes", sessionId);
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

    private async Task<string> StartRaceContainer(string sessionId)
    {
        string container = string.Empty;
        try
        {
            logger.LogInformation("[session: {sessionId}]: Starting container for session", sessionId);
            var containerRes = await dockerClient.Containers.CreateContainerAsync(new()
            {
                Image = "zickbalazs/api-example-node:dev"
            });
            await dockerClient.Containers.StartContainerAsync(containerRes.ID, new());
            container = containerRes.ID;
        }
        catch (Exception e)
        {
            logger
                .LogCritical("[session: {sessionId}]: Failed to start container with\nmessage: {error}\nStack trace: {stack}", 
                    sessionId,
                    e.Message,
                    e.StackTrace);
        }
        return container;
    }

    private async Task UpdateSessionToStatus(string sessionId, SessionStatus status)
    {
        try
        {
            var session = ctx.Sessions.First(x => x.Id == new Guid(sessionId));
            session.Status = status;
            await ctx.SaveChangesAsync();
        }
        catch (Exception e)
        {
            if (e is ArgumentNullException or InvalidOperationException)
                logger.LogCritical("[session: {sessionId}]: Session update failed due to the session not existing", sessionId);
            else    
                logger.LogCritical("[session: {sessionId}]: Session update failed due to a database error\nmessage: {message}\nstack: {stack}",
                    sessionId,
                    e.Message,
                    e.StackTrace);
        }
    }

    
    private async Task CreateWatchJob(string sessionId, string containerId)
    {
        var watchJob = JobBuilder
            .Create<WatchSessionJob>()
            .WithIdentity(sessionId, "watch-session")
            .WithDescription($"containerId:{containerId}")
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
        logger.LogInformation("[session: {sessionId}]: Starting watch job for container", sessionId);
    }
    
    
    
}