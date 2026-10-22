using Microsoft.EntityFrameworkCore;
using Prometheus;
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
    private readonly Counter _cancelledSessionMetric = 
        Metrics.CreateCounter("sessions_cancelled_total", "Total sessions that were cancelled");
    private readonly Counter _scheduledSessionMetric =
        Metrics.CreateCounter("sessions_scheduled_total", "Total sessions that were scheduled for start");
    private readonly Counter _lateSessionMetric =
        Metrics.CreateCounter("sessions_late_total", "Total sessions that were late but within the acceptable delta");
    
    
    
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            logger.LogInformation("started session collection");
            await DoWork();
            logger.LogInformation("stopped session collection");
            await Task.Delay(60000, stoppingToken);
        }
    }

    private async Task DoWork()
    {
        IList<LeagueSession<RacingUserIdentity>> sessions;
        await using (var ctx = await ctxFactory.CreateDbContextAsync())
        {
            await CancelOldSessions(ctx);
            sessions = await EnqueueSessions(ctx);
        }
        foreach (var session in sessions)
        {
            await CreateJobWithTrigger(session);
        }
    }

    private async Task CancelOldSessions(ApplicationContext<RacingUserIdentity> ctx)
    {
        DateTime tooLate = DateTime.Now.AddMinutes(-30);

        var sessionsCount = await ctx.Sessions
            .Where(x => x.Start < tooLate && x.Status==SessionStatus.Created)
            .ExecuteUpdateAsync(x => x.SetProperty(n => n.Status, SessionStatus.Cancelled));

        if (sessionsCount > 0)
        {
            logger.LogWarning("cancelled {lateSessionCount} sessions for being too late", sessionsCount);
            _cancelledSessionMetric.Inc(sessionsCount);
        }
    }
    
    private async Task<IList<LeagueSession<RacingUserIdentity>>> EnqueueSessions(ApplicationContext<RacingUserIdentity> ctx)
    {
        DateTime signDelta = DateTime.Now.AddHours(2);
        var sessionsCount = ctx.Sessions
            .Where(x => x.Start < signDelta && x.Status == SessionStatus.Created)
            .OrderBy(x => x.Start)
            .Take(config.GetValue<int>("SessionProcessor:BatchLimit"));
        logger.LogInformation("found {sessionCount} for scheduling", sessionsCount.Count());
        var r = sessionsCount.ToList();
        await sessionsCount.ExecuteUpdateAsync(x => x.SetProperty(n => n.Status, SessionStatus.Waiting));
        return r;
    }

    private async Task CreateJobWithTrigger(LeagueSession<RacingUserIdentity> session)
    {
        var job = JobBuilder
            .Create<StartSessionJob>()
            .WithIdentity(session.Id.ToString(), "start-jobs")
            .Build();

        bool wasLate = false;
        
        DateTime now = DateTime.Now;
        DateTime? plannedStart = null;

        if (session.Start < now)
        {
            plannedStart = DateTime.Now + TimeSpan.FromMinutes(5);
            await using (var ctx = await ctxFactory.CreateDbContextAsync())
            {
                ctx.Sessions.First(x => x.Id == session.Id).Start = plannedStart ?? DateTime.Now;
                await ctx.SaveChangesAsync();
            }
            logger.LogWarning("{sessionId} session start is late, planned start was {sessionStartTime} actual start is {actualStartTime}",
                session.Id,
                session.Start,
                plannedStart);
            wasLate = true;
        }
        
        var trigger = TriggerBuilder
            .Create()
            .ForJob(job)
            .WithIdentity(session.Id.ToString(), "start-triggers")
            .StartAt(plannedStart ?? session.Start).Build();

        var scheduler = await schedulerFactory.GetScheduler();
        await scheduler.ScheduleJob(job, trigger);

        if (wasLate)
            _lateSessionMetric.Inc();
        else
            _scheduledSessionMetric.Inc();
    }
}