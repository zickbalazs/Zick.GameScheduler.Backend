using Microsoft.EntityFrameworkCore;
using Quartz;
using Zick.GameScheduler.Backend.Data;
using Zick.GameScheduler.Backend.Data.Models;
using Zick.GameScheduler.Backend.Scheduler.Jobs;

namespace Zick.GameScheduler.SessionWorker;

public class SessionProcessorWorker(ILogger<SessionProcessorWorker> logger,
    IConfiguration config,
    IDbContextFactory<ApplicationContext<RacingUserIdentity>> ctxFactory,
    ISchedulerFactory schedulerFactory) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            logger.LogInformation("started session collection");
            await DoWork();
            await Task.Delay(15000, stoppingToken);
        }
    }

    private async Task DoWork()
    {
        LeagueSession<RacingUserIdentity>[] newSessions;
        
        await using (var ctx = await ctxFactory.CreateDbContextAsync())
        {
            var sessionsToProcess = ctx.Sessions
                .Include(x=>x.League)
                .Where(SessionIsInADayAndIsCreated)
                .Take(config.GetValue<int>("SessionProcessor:BatchLimit"));

            newSessions = [ ..sessionsToProcess ];
            
            logger.LogInformation("found {sessionCount} sessions for processing", newSessions.Length);
        }
        
        if (newSessions.Length < 1)
            return;

        await using (var ctx = await ctxFactory.CreateDbContextAsync())
        {
            foreach (var session in newSessions)
            {
                var dbEntry = await ctx.Sessions.FirstAsync(x => x.Id == session.Id);
                dbEntry.Status = SessionStatus.Waiting;
            }
            await ctx.SaveChangesAsync();
        }

        foreach (var session in newSessions)
        {
            await CreateJobWithTrigger(session);
        }
    }

    private bool SessionIsInADayAndIsCreated(LeagueSession<RacingUserIdentity> session)
    {
        return session.Start < DateTime.Now + TimeSpan.FromDays(1) && session.Status == SessionStatus.Created;
    }
    
    
    private async Task CreateJobWithTrigger(LeagueSession<RacingUserIdentity> session)
    {
        var job = JobBuilder
            .Create<StartSessionJob>()
            .WithIdentity(session.Id.ToString(), "start-jobs")
            .Build();

        DateTime now = DateTime.Now;
        DateTime? plannedStart = null;
        
        
        if (session.Start < now)
        {
            plannedStart = DateTime.Now + TimeSpan.FromMinutes(2);
            logger.LogWarning("{sessionId} session start is late, planned start was {sessionStartTime} actual start is {actualStartTime}",
                session.Id,
                session.Start,
                plannedStart);
        }
        
        
        var trigger = TriggerBuilder
            .Create()
            .ForJob(job)
            .WithIdentity(session.Id.ToString(), "start-triggers")
            .StartAt(plannedStart ?? session.Start).Build();

        var scheduler = await schedulerFactory.GetScheduler();
        await scheduler.ScheduleJob(job, trigger);
    }
    
}