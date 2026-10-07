using Docker.DotNet;
using Docker.DotNet.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Zick.GameScheduler.Backend.Data;
using Zick.GameScheduler.Backend.Data.Models;

namespace Zick.GameScheduler.Backend.Scheduler.Services;

// ONLY USE IN DEV
public class DockerEngineContainerService(IDockerClient dockerClient,
    IPortClaimService portClaimService,
    ILogger<DockerEngineContainerService> logger,
    IConfiguration config) : IContainerService
{
    public async Task<string> StartContainerForSession(Guid sessionId)
    {
        var ports = (await portClaimService.GetPortsForSession(sessionId)).Split(';');
        
        string serverPort = ports[0];
        string httpPort = ports[1];
        
        var containerRes = await dockerClient.Containers.CreateContainerAsync(new()
        {
            Image = config.GetValue<string>("Docker:ServerImage"),
            //TODO: Env, Ports
        });

        await dockerClient.Containers.StartContainerAsync(containerRes.ID, new());

        logger.LogInformation("[session: {sessionId}]: Started container for session with containerId: {containerId}",
            sessionId, containerRes.ID);
        return containerRes.ID;
    }

    private void CreateContainerPortParams()
    {
        //TODO: Move HostConfig creation here.
    }

    private void CreateContainerEnv()
    {
        //TODO: Env variable settings here.
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