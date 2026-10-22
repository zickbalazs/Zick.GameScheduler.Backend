using System.Text;
using Docker.DotNet;
using Docker.DotNet.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Zick.GameScheduler.Backend.Data;
using Zick.GameScheduler.Backend.Data.Models;
using Zick.GameScheduler.Services.Generators;

namespace Zick.GameScheduler.Backend.Scheduler.Services;

public class DockerSwarmContainerService(IDockerClient client,
    IConfiguration config,
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

        var tasks = await client.Tasks.ListAsync();

        var task = tasks.First(x=>x.ServiceID == serviceResponse.ID);

        await Task.Delay(10000);

        return serviceResponse.ID;
    }

    public async Task StopContainerForSession(Guid sessionId, string containerId)
    {
        var tasks = await client.Tasks.ListAsync();
        await ExtractResults(sessionId, 
            tasks.First(x => x.ServiceID == containerId).Status.ContainerStatus.ContainerID);
        await client.Swarm.RemoveServiceAsync(containerId);
        await DeleteConfForSession(sessionId);
    }

    public Task RecoverContainerForSession(Guid sessionId, string containerId)
    {
        throw new NotImplementedException();
    }

    public async Task DeleteConfForSession(Guid sessionId)
    {
        await client.Configs.RemoveConfigAsync($"{sessionId}");
    }

    private async Task ExtractResults(Guid sessionId, string containerId)
    {
        var archive = await client.Containers.GetArchiveFromContainerAsync(containerId, new()
        {
            Path = "/opt/app/assetto-dedicated/server/results" 
        }, false);

        string path = Path.Join(config.GetValue<string>("Results:Path"), sessionId.ToString(), "results.tar");
        
        
        using (FileStream fileStream = new FileStream(path, FileMode.OpenOrCreate))
        {
            await archive.Stream.CopyToAsync(fileStream);
        }
    }
    
    
    private async Task<string> CreateEntryListForSession(Guid sessionId, string entryListContents)
    {
        var configId = await client.Configs.CreateConfigAsync(new()
        {
            Config = new SwarmConfigSpec()
            {
                Name = $"{sessionId}",
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
            .ThenInclude(x=>x.CurrentTrack)
            .Include(n=>n.Track)
            .Include(x=>x.Registrations)
            .ThenInclude(m=>m.Car)
            .FirstAsync(x => x.Id == sessionId);

        string serverPass = sessionEntry.GetHashCode().ToString();

        await portClaimService.AddPasswordForSession(sessionEntry.PortClaim!.Id, serverPass);

        int pracTime = sessionEntry.League.Practice == null ? 0 : sessionEntry.League.Practice.DurationInMinutes ?? 0;
        int qualyTime = sessionEntry.League.Qualify == null ? 0 : sessionEntry.League.Qualify.DurationInMinutes ?? 0;
        int raceTime = sessionEntry.League.Race == null ? 0 : sessionEntry.League.Race.DurationInMinutes ?? 0;
        int raceLaps = sessionEntry.League.Race == null ? 0 : sessionEntry.League.Race.Laps ?? 0;
        
        spec.TaskTemplate.ContainerSpec.Env = new List<string>()
        {
            $"PRAC_TIME={pracTime}",
            $"QUALY_TIME={qualyTime}",
            $"RACE_TIME={raceTime}",
            $"RACE_LAPS={raceLaps}",
            $"VEHICLES={string.Join(";",sessionEntry.Registrations.Select(x=>x.Car.FolderName).Distinct())}",
            $"SERVER_NAME='Zick GameScheduler {sessionId}'",
            $"TRACK={sessionEntry.Track?.FolderName ?? sessionEntry.League.CurrentTrack.FolderName}",
            $"SERVER_PASSWORD={serverPass}",
            $"SERVER_PORT={serverPort}",
            $"HTTP_PORT={httpPort}",
            $"MAX_PARTICIPANTS={sessionEntry.Registrations.Count}"
        };

        return spec;
    }
}