using Docker.DotNet;
using Microsoft.Extensions.Logging;
using Quartz;
using Zick.GameScheduler.Backend.Data;
using Zick.GameScheduler.Backend.Data.Models;

namespace Zick.GameScheduler.Backend.Scheduler.Jobs;

public class StartSessionJob(ILogger<StartSessionJob> logger, 
    ApplicationContext<RacingUserIdentity> ctx, 
    IDockerClient dockerClient) : IJob
{
    public async Task Execute(IJobExecutionContext context)
    {
        logger.LogInformation("Locking registrations and starting server for session: {sessionId}", context.JobDetail.Key.Name);

        var container = await dockerClient.Containers.CreateContainerAsync(new()
        {
            Name = $"assetto-server-{context.JobDetail.Key.Name}",
            Image = "zickbalazs/assetto-dedicated-server",
        });
    }
}