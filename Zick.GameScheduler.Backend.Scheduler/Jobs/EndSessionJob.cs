using Microsoft.Extensions.Logging;
using Quartz;
using Zick.GameScheduler.Backend.Scheduler.Services;
using Zick.GameScheduler.Services.Generators;

namespace Zick.GameScheduler.Backend.Scheduler.Jobs;

public class EndSessionJob(IContainerService containerService,
    IPortClaimService claimService,
    IResultConsumerService resultConsumer,
    ILogger<EndSessionJob> logger) : IJob
{
    public async Task Execute(IJobExecutionContext context)
    {
        Guid sessionId = new Guid(context.JobDetail.Key.Name);
        string containerId = context.JobDetail.Description!;


        
        await claimService.EndClaimForSession(sessionId);
        //await resultConsumer.ConsumeAndUploadResult(sessionId, "");
        await containerService.StopContainerForSession(sessionId, containerId);
        




    }
}