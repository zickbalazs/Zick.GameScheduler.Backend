using Docker.DotNet;
using Microsoft.Extensions.Logging;
using Quartz;

namespace Zick.GameScheduler.Backend.Scheduler.Jobs;

public class WatchSessionJob(ILogger<WatchSessionJob> logger,
    IDockerClient dockerClient) : IJob
{
    public async Task Execute(IJobExecutionContext context)
    {
        string containerId = context.JobDetail.Description!.Split(':')[1], 
               sessionId = context.JobDetail.Key.Name;
        logger.LogTrace("[session: {sessionId}]: watching container with id {containerId}, repetition count: {watchCount}", 
            sessionId, 
            containerId,
            ((ISimpleTrigger)context.Trigger).TimesTriggered);
        
        
        
        


        if (await CancelConditionSatisfied(context))
        {
            await context.Scheduler.DeleteJob(context.JobDetail.Key);
            logger.LogInformation("[session: {sessionId}]: finishing job, race has ended", sessionId);
        }
    }






    private async Task<bool> CancelConditionSatisfied(IJobExecutionContext context)
    {
        return ((ISimpleTrigger)context.Trigger).TimesTriggered > 5;
    }
}