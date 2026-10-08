using System.Security.Cryptography;
using System.Text;
using Docker.DotNet;
using Docker.DotNet.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Zick.GameScheduler.Backend.Data;
using Zick.GameScheduler.Backend.Data.Models;
using Zick.GameScheduler.Backend.EntryListGenerator;

namespace Zick.GameScheduler.Backend.Scheduler.Services;

// ONLY USE IN DEV
public class DockerEngineContainerService(IDockerClient dockerClient,
    ApplicationContext<RacingUserIdentity> ctx,
    IPortClaimService portClaimService,
    ILogger<DockerEngineContainerService> logger,
    IEntryGenerator<RacingUserIdentity> entryGenerator,
    IConfiguration config) : IContainerService
{
    public async Task<string> StartContainerForSession(Guid sessionId)
    {
        var ports = (await portClaimService.GetPortsForSession(sessionId)).Split(';');

        var sessionLeagueData = ctx.Sessions
            .Include(x => x.League)
            .First(x=>x.Id == sessionId)
            .League;
        
        string serverPort = ports[0];
        string httpPort = ports[1];

        var entryList = await entryGenerator.GenerateEntryListForSessions(sessionId);

        await CreateEntryListForSession(sessionId, entryList);
        
        var containerRes = await dockerClient.Containers.CreateContainerAsync(new()
        {
            Image = config.GetValue<string>("Docker:ServerImage"),
            ExposedPorts = CreateContainerPortParams(serverPort, httpPort),
            HostConfig = CreateContainerHostConfig(serverPort, httpPort),
            Env = CreateContainerEnv(sessionId,
                practice: sessionLeagueData.Practice, 
                qualifying: sessionLeagueData.Qualify, 
                race: sessionLeagueData.Race,
                hostPort: serverPort,
                httpPort: httpPort),
        });

        await dockerClient.Containers.StartContainerAsync(containerRes.ID, new());

        logger.LogInformation("[session: {sessionId}]: Started container for session with containerId: {containerId}",
            sessionId, containerRes.ID);
        return containerRes.ID;
    }

    private Dictionary<string, EmptyStruct> CreateContainerPortParams(string hostPort, string httpPort)
    {
        return new()
        {
            { $"{hostPort}/tcp", default },
            { $"{httpPort}/udp", default },
            { $"{httpPort}/tcp", default }
        };
    }

    private HostConfig CreateContainerHostConfig(string hostPort, string httpPort)
    {
        return new()
        {
            PortBindings = new Dictionary<string, IList<PortBinding>>()
            {
                { $"{hostPort}/tcp", [new() { HostPort = hostPort }] },
                { $"{hostPort}/udp", [new() { HostPort = hostPort }] },
                { $"{httpPort}/tcp", [new() { HostPort = httpPort }] }
            }
        };
    }

    private List<string> CreateContainerEnv(Guid sessionId, 
        SessionData? practice,
        SessionData? qualifying,
        SessionData? race,
        string hostPort,
        string httpPort)
    {
        return [ 
            $"SERVER_NAME=\"Zick's GameScheduler: {sessionId}\"",
            $"PRAC_TIME={practice?.DurationInMinutes}", 
            $"PRAC_LAPS={practice?.Laps}", 
            $"QUALY_LAPS={qualifying?.DurationInMinutes}",
            $"QUALY_TIME={qualifying?.DurationInMinutes}",
            $"RACE_TIME={race?.DurationInMinutes}",
            $"RACE_LAPS={race?.Laps}",
            $"SERVER_PORT={hostPort}",
            $"HTTP_PORT={httpPort}",
            $"SERVER_PASSWORD={GeneratePasswordForSession(sessionId)}"
        ];
    }

    private string GeneratePasswordForSession(Guid id)
    {
        return BitConverter.ToString(SHA256.HashData(Encoding.UTF8.GetBytes($"session-{id}-zgs")));
    }
    
    
    public Task StopContainerForSession(Guid sessionId, string containerId)
    {
        throw new NotImplementedException();
    }

    public Task RecoverContainerForSession(Guid sessionId, string containerId)
    {
        throw new NotImplementedException();
    }

    public async Task CreateEntryListForSession(Guid sessionId, string entryListContents)
    {
        var configId = await dockerClient.Configs.CreateConfigAsync(new()
        {
            Config = new SwarmConfigSpec()
            {
                Name = $"{sessionId}-initial",
                Data = Encoding.UTF8.GetBytes(entryListContents)
            }
        });

        logger.LogInformation("[session: {sessionId}]: created configuration for session with config id: {configId}",
            sessionId, configId.ID);
    }
}