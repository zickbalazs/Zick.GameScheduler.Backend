using Microsoft.Extensions.Logging;
using Quartz;

namespace Zick.GameScheduler.Backend.Scheduler.Jobs;

public class SessionJob(ILogger<SessionJob> logger) : IJob
{
    public async Task Execute(IJobExecutionContext context)
    {
        logger.LogInformation("Starting session id: {sessionId}", context.JobDetail.Key);
    }
}