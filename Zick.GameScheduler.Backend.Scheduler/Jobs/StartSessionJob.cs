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

        await using (var ctx = await contextFactory.CreateDbContextAsync())
        {
            logger.LogInformation("[startJob | session: {sessionId}]: checking registration count", sessionId);
            var session = ctx.Sessions
                .Include(x=>x.Registrations)
                .First(x => x.Id == sessionId);

            if (session.Registrations.Count < 2)
            {
                logger.LogWarning(
                    "[startJob | session: {sessionId}]: stopping startup for session, not enough registrations",
                    sessionId);
                session.Status = SessionStatus.Cancelled;
                await ctx.SaveChangesAsync();
                return;
            }
        }
        
        await updateService.UpdateSessionWithStatus(sessionId, SessionStatus.Starting);
        logger.LogInformation("[startJob | session: {sessionId}]: Pre-checks done, starting job for session", sessionId);
        
        var ports = (await portClaimService.ClaimPortForSession(sessionId)).Split(';');
        
        var containerId = await containerService.StartContainerForSession(sessionId);

        await CreateWatchJob(containerId, sessionId);
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
            .StartNow()
            .ForJob(job)
            .Build();
        var scheduler = await schedFactory.GetScheduler();

        await scheduler.ScheduleJob(job, trigger);
    }
}