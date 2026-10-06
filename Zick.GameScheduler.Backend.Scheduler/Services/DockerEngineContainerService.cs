using Docker.DotNet;
using Microsoft.Extensions.Configuration;
using Zick.GameScheduler.Backend.Data;
using Zick.GameScheduler.Backend.Data.Models;

namespace Zick.GameScheduler.Backend.Scheduler.Services;

// ONLY USE IN DEV
public class DockerEngineContainerService(IDockerClient dockerClient,
    IConfiguration config,
    IPortClaimService claimService) : IContainerService
{
    public async Task StartContainerForSession(Guid sessionId)
    {
        
    }

    public Task StopContainerForSession(Guid sessionId, string containerId)
    {
        throw new NotImplementedException();
    }

    public Task RecoverContainerForSession(Guid sessionId, string containerId)
    {
        throw new NotImplementedException();
    }
}