using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Quartz;
using Zick.GameScheduler.Backend.Data;
using Zick.GameScheduler.Backend.Data.Models;
using Zick.GameScheduler.Backend.Scheduler.Jobs;

namespace Zick.GameScheduler.Backend.Scheduler.Services;

public class QuartzSchedulerService(ApplicationContext<RacingUserIdentity> ctx,
    ILogger<QuartzSchedulerService> logger,
    ISchedulerFactory scheduler) : ISessionSchedulerService
{
    public async Task CreateSessionFor(Guid sessionId)
    {
        logger.LogTrace("Starting job for session with id: {sessionId}", sessionId);

        try
        {
            var session = await ctx.Sessions
                .FirstOrDefaultAsync(x => x.Id == sessionId);
            
            if (session is null)
                logger.LogError("session is not found in db");
            else
            {
                logger.LogInformation("found session in db, sending job for scheduler");
                var job = JobBuilder.Create<StartSessionJob>()
                    .WithIdentity($"{sessionId}", "session-wait-jobs")
                    .Build();
                var trigger = TriggerBuilder.Create()
                    .ForJob(job)
                    .StartAt(session.Start.AddMinutes(-5))
                    .Build();

                await (await scheduler.GetScheduler()).ScheduleJob(job, trigger);
            }
        }
        catch (Exception e)
        {
            logger
                .LogError("job failed with error type of {errorType} and with message: {errorMessage}, cancelling race for {sessionId}", 
                    e.GetType().Name, 
                    e.Message,
                    sessionId);
            var session = ctx.Sessions.First(x => x.Id == sessionId);
            session.Status = SessionStatus.Error;
            await ctx.SaveChangesAsync();
        }
        
        
    }

    public Task CreateWatcherForSession(Guid sessionId)
    {
        throw new NotImplementedException();
    }
}