using Microsoft.Extensions.Logging;
using Quartz;
using Zick.GameScheduler.Backend.Data.Models;
using Zick.GameScheduler.Backend.Scheduler.Services;
using Zick.GameScheduler.Services.Generators;

namespace Zick.GameScheduler.Backend.Scheduler.Jobs;

public class EndSessionJob(
    IPortClaimService claimService,
    ISessionStatusUpdateService updateService,
    ILogger<EndSessionJob> logger) : IJob
{
    public async Task Execute(IJobExecutionContext context)
    {
        string stopReason = "success";
        Guid sessionId = new Guid(context.JobDetail.Key.Name);
        string containerId = context.JobDetail.Description!;
        logger.LogInformation("[endJob | session {sessionId}]: start end job", sessionId);
        
        await claimService.EndClaimForSession(sessionId);
        await updateService.UpdateSessionWithStatus(sessionId, SessionStatus.Finished);
        
        logger.LogInformation("[endJob | session {sessionId}]: stop end job with reason: {endReason}", 
            sessionId,
            stopReason);
    }
}