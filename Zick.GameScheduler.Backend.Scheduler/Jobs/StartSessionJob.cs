using Docker.DotNet;
using Microsoft.Extensions.Logging;
using Quartz;
using Zick.GameScheduler.Backend.Data;
using Zick.GameScheduler.Backend.Data.Models;
using Zick.GameScheduler.Backend.Scheduler.Services;

namespace Zick.GameScheduler.Backend.Scheduler.Jobs;

public class StartSessionJob(ILogger<StartSessionJob> logger,
    IContainerService containerService,
    IPortClaimService portClaimService,
    ISchedulerFactory schedFactory) : IJob
{
    public async Task Execute(IJobExecutionContext context)
    {
        Guid sessionId = new Guid(context.JobDetail.Key.Name);
        
        logger.LogInformation("[startJob | session: {sessionId}]: starting job for session", sessionId);
        var ports = await portClaimService.ClaimPortForSession(sessionId);






    }
}