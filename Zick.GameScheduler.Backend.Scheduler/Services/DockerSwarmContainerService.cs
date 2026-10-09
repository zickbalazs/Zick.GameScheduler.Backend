using System.Text;
using Docker.DotNet;
using Docker.DotNet.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Zick.GameScheduler.Backend.Data;
using Zick.GameScheduler.Backend.Data.Models;
using Zick.GameScheduler.Services.Generators;

namespace Zick.GameScheduler.Backend.Scheduler.Services;

public class DockerSwarmContainerService(IDockerClient client,
    IPortClaimService portClaimService,
    IEntryGenerator<RacingUserIdentity> entryGenerator, 
    ApplicationContext<RacingUserIdentity> ctx,
    ILogger<DockerSwarmContainerService> logger) : IContainerService
{
    public async Task<string> StartContainerForSession(Guid sessionId)
    {
        //TODO: Start watch job, error handling
        var ports = (await portClaimService.ClaimPortForSession(sessionId)).Split(';');
        var entryList = await entryGenerator.GenerateEntryListForSessions(sessionId);
        var configId = await CreateEntryListForSession(sessionId, entryList);
        var serviceSpec = Utils.SwarmUtils.CreateSpec(sessionId, configId, ports[0], ports[1]);
        serviceSpec = await CreateEnvForServiceSpec(sessionId, serviceSpec, ports[0], ports[1]);
        
        
        var serviceResponse = await client.Swarm
            .CreateServiceAsync(new(){
                Service = serviceSpec
            });

        logger.LogInformation("[session {sessionId}]: Started service for race with id {serviceId}", 
            sessionId,
            serviceResponse.ID);
        
        return serviceResponse.ID;
    }

    public Task StopContainerForSession(Guid sessionId, string containerId)
    {
        throw new NotImplementedException();
    }

    public Task RecoverContainerForSession(Guid sessionId, string containerId)
    {
        throw new NotImplementedException();
    }

    private async Task<string> CreateEntryListForSession(Guid sessionId, string entryListContents)
    {
        var configId = await client.Configs.CreateConfigAsync(new()
        {
            Config = new SwarmConfigSpec()
            {
                Name = $"{sessionId}-initial",
                Data = Encoding.UTF8.GetBytes(entryListContents)
            }
        });

        logger.LogInformation("[session: {sessionId}]: created configuration for session with config id: {configId}",
            sessionId, configId.ID);

        return configId.ID;
    }

    private async Task<ServiceSpec> CreateEnvForServiceSpec(Guid sessionId,
        ServiceSpec spec,
        string serverPort,
        string httpPort)
    {
        var sessionEntry = await ctx.Sessions
            .Include(x=>x.League)
            .Include(n=>n.Track)
            .Include(x=>x.Registrations)
            .ThenInclude(m=>m.Car)
            .FirstAsync(x => x.Id == sessionId); 
        
        spec.TaskTemplate.ContainerSpec.Env = new List<string>()
        {
            $"PRAC_TIME={sessionEntry.League.Practice?.DurationInMinutes}",
            $"VEHICLES={string.Join(";",sessionEntry.Registrations.Select(x=>x.Car.FolderName).Distinct())}",
            $"SERVER_NAME='Zick GameScheduler {sessionId}'",
            $"TRACK={sessionEntry.Track?.FolderName ?? sessionEntry.League.CurrentTrack.FolderName}",
            $"SERVER_PASSWORD={sessionEntry.GetHashCode()}",
            $"SERVER_PORT={serverPort}",
            $"HTTP_PORT={httpPort}",
            $"MAX_PARTICIPANTS={sessionEntry.Registrations.Count}"
        };

        return spec;
    }
}