using Docker.DotNet;
using Microsoft.Extensions.Logging;
using Quartz;
using Zick.GameScheduler.Backend.Scheduler.Services;

namespace Zick.GameScheduler.Backend.Scheduler.Jobs;

public class WatchSessionJob(ILogger<WatchSessionJob> logger, IContainerService containerService) : IJob
{
    public async Task Execute(IJobExecutionContext context)
    {
        
    }
}