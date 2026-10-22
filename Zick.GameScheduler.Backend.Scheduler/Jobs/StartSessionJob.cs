using Docker.DotNet;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Quartz;
using Zick.GameScheduler.Backend.Data;
using Zick.GameScheduler.Backend.Data.Models;
using Zick.GameScheduler.Backend.Scheduler.Services;

namespace Zick.GameScheduler.Backend.Scheduler.Jobs;

public class StartSessionJob(ILogger<StartSessionJob> logger,
    IContainerService containerService,
    ISessionStatusUpdateService updateService,
    IPortClaimService portClaimService,
    IDbContextFactory<ApplicationContext<RacingUserIdentity>> contextFactory,
    ISchedulerFactory schedFactory) : IJob
{
    public async Task Execute(IJobExecutionContext context)
    {
        Guid sessionId = new Guid(context.JobDetail.Key.Name);
        logger.LogInformation("[startJob | session: {sessionId}]: job start", sessionId);

        await using (var ctx = await contextFactory.CreateDbContextAsync())
        {
            var session = ctx.Sessions
                .Include(x=>x.Registrations)
                .Include(x=>x.League)
                .ThenInclude(x=>x.CurrentTrack)
                .First(x => x.Id == sessionId);

            if (session.Registrations.Count < 2)
            {
                await updateService.UpdateSessionWithStatus(sessionId, SessionStatus.Cancelled);
                logger.LogWarning(
                    "[startJob | session: {sessionId}]: stopping startup for session, not enough registrations",
                    sessionId);
                return;
            }

            if (session.Status == SessionStatus.Cancelled)
            {
                logger.LogWarning("[startJob | session: {sessionId}]: session was cancelled while waiting",
                    sessionId);
                return;
            }

            session.Track = session.League.CurrentTrack;
            await ctx.SaveChangesAsync();
        }
        
        await updateService.UpdateSessionWithStatus(sessionId, SessionStatus.Starting);

        try
        {
            var containerId = await containerService.StartContainerForSession(sessionId);
            logger.LogInformation("[startJob | session: {sessionId}]: session start succeeded, starting watch job",
                sessionId);
            await updateService.UpdateSessionWithStatus(sessionId, SessionStatus.Underway);
            await CreateWatchJob(containerId, sessionId);
        }
        catch (Exception e)
        {
            await updateService.UpdateSessionWithStatus(sessionId, SessionStatus.Error);
            await portClaimService.EndClaimForSession(sessionId);
            await containerService.DeleteConfForSession(sessionId);
            logger.LogError(e, "[startJob | session: {sessionId}]: Session start failed", sessionId);
        }
        finally
        {
            logger.LogInformation("[startJob | session: {sessionId}]: start job end", sessionId);
        }
    }

    private async Task CreateWatchJob(string containerId, Guid sessionId)
    {
        var job = JobBuilder
            .Create<WatchSessionJob>()
            .WithIdentity(sessionId.ToString(), "watch-job")
            .WithDescription(containerId)
            .Build();
        var trigger = TriggerBuilder
            .Create()
            .StartAt(DateTime.Now.AddSeconds(45))
            .ForJob(job)
            .WithSimpleSchedule(x=>x.RepeatForever().WithIntervalInSeconds(45))
            .Build();
        var scheduler = await schedFactory.GetScheduler();

        await scheduler.ScheduleJob(job, trigger);
    }
}