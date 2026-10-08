using Docker.DotNet;
using Docker.DotNet.Models;
using Microsoft.Extensions.Logging;
using Zick.GameScheduler.Backend.Data.Models;
using Zick.GameScheduler.Backend.EntryListGenerator;

namespace Zick.GameScheduler.Backend.Scheduler.Services;

public class DockerSwarmContainerService(IDockerClient client,
    IPortClaimService portClaimService,
    IEntryGenerator<RacingUserIdentity> entryGenerator, 
    ILogger<DockerSwarmContainerService> logger) : IContainerService
{
    public async Task<string> StartContainerForSession(Guid sessionId)
    {
        // TODO
        var ports = (await portClaimService.ClaimPortForSession(sessionId)).Split(';');
        var config = await entryGenerator.GenerateEntryListForSessions(sessionId);
        
        
        var response = await client.Swarm
            .CreateServiceAsync(new()
            {
                Service = Utils.SwarmUtils.CreateSpec(sessionId, config)
            });

        logger.LogInformation("[session {sessionId}]: Started service for race with id {serviceId}", 
            sessionId,
            response.ID);
        
        
        return response.ID;
    }

    public Task StopContainerForSession(Guid sessionId, string containerId)
    {
        throw new NotImplementedException();
    }

    public Task RecoverContainerForSession(Guid sessionId, string containerId)
    {
        throw new NotImplementedException();
    }

    public Task CreateEntryListForSession(Guid sessionId, string entryListContents)
    {
        throw new NotImplementedException();
    }
}