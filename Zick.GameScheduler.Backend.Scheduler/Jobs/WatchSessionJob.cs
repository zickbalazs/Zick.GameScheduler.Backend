using Docker.DotNet;
using Microsoft.Extensions.Logging;
using Quartz;
using Zick.GameScheduler.Backend.Scheduler.Services;

namespace Zick.GameScheduler.Backend.Scheduler.Jobs;

public class WatchSessionJob(IDockerClient dockerClient,
    ISchedulerFactory schedulerFactory,
    ILogger<WatchSessionJob> logger) : IJob
{
    public async Task Execute(IJobExecutionContext context)
    {
        var sessionId = context.JobDetail.Key.Name;
        var serviceId = context.JobDetail.Description!;
        logger.LogInformation("watching session: {sessionId}", sessionId);
    }
}