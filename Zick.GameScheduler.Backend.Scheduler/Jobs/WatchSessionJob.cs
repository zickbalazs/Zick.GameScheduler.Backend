using Docker.DotNet;
using Docker.DotNet.Models;
using Microsoft.Extensions.Logging;
using Quartz;
using Zick.GameScheduler.Backend.Data.Models;
using Zick.GameScheduler.Backend.Scheduler.Services;

namespace Zick.GameScheduler.Backend.Scheduler.Jobs;

public class WatchSessionJob(IDockerClient dockerClient,
    ISchedulerFactory schedulerFactory,
    ISessionStatusUpdateService updateService,
    ILogger<WatchSessionJob> logger) : IJob
{
    public async Task Execute(IJobExecutionContext context)
    {
        var sessionId = context.JobDetail.Key.Name;
        var serviceId = context.JobDetail.Description!;
        
        logger.LogDebug("watching session: {sessionId}", sessionId);

        TaskResponse jobTask;
        
        try
        {
            IList<TaskResponse> tasks = await dockerClient.Tasks.ListAsync();
            jobTask = tasks.First(x => x.ServiceID == serviceId);
        }
        catch (Exception e)
        {
            await updateService.UpdateSessionWithStatus(new Guid(sessionId), SessionStatus.Error);
            await CancelWatchJob(context);
            logger.LogCritical(e, "[watchJob | session {sessionId}]: a critical error occured", sessionId);
            return;
        }
        
        
        if (jobTask.Status.State == TaskState.Running)
        {
            logger.LogInformation("[watchJob | session {sessionId}]: container healthy, continuing", sessionId);
            return;
        }

        logger.LogInformation("[watchJob | session {sessionId}]: container stopped, scheduling stop job", sessionId);
        await ScheduleStopJob(sessionId, serviceId);
        await CancelWatchJob(context);
    }

    private async Task ScheduleStopJob(string sessionId, string serviceId)
    {
        var job = JobBuilder
            .Create<EndSessionJob>()
            .WithIdentity(sessionId, "end-jobs")
            .WithDescription(serviceId)
            .Build();

        var trigger = TriggerBuilder
            .Create()
            .ForJob(job)
            .StartAt(DateTime.Now.AddSeconds(45))
            .Build();

        var scheduler = await schedulerFactory.GetScheduler();
        await scheduler.ScheduleJob(job, trigger);
    }

    private async Task CancelWatchJob(IJobExecutionContext ctx)
    {
        await (await schedulerFactory.GetScheduler()).DeleteJob(ctx.JobDetail.Key);
    }
    
}