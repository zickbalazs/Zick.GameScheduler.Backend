using Docker.DotNet;
using Docker.DotNet.Models;
using Microsoft.Extensions.Logging;

namespace Zick.GameScheduler.Backend.Scheduler.Services;

public class DockerSwarmContainerService(//IDockerClient client,
    IPortClaimService portClaimService,
    ILogger<DockerSwarmContainerService> logger) : IContainerService
{
    public async Task StartContainerForSession(Guid sessionId)
    {
        var ports = (await portClaimService.ClaimPortForSession(sessionId)).Split(';');
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