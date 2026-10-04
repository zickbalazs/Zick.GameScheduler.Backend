using Microsoft.Extensions.Logging;
using Quartz;

namespace Zick.GameScheduler.Backend.Scheduler.Jobs;

public class WatchSessionJob(ILogger<WatchSessionJob> logger) : IJob
{
    public async Task Execute(IJobExecutionContext context)
    {
        string containerId = context.JobDetail.Description!.Split(':')[1], 
               sessionId = context.JobDetail.Key.Name;
        logger.LogInformation("watching container {containerId} for session {sessionId}", containerId, sessionId);



        if (((ISimpleTrigger)context.Trigger).TimesTriggered > 5)
        {
            await context.Scheduler.DeleteJob(context.JobDetail.Key);
            logger.LogInformation("finishing job for {sessionId}, race has ended", sessionId);
        }
    }
}