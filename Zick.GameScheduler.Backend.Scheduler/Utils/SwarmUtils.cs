using Docker.DotNet.Models;

namespace Zick.GameScheduler.Backend.Scheduler.Utils;

public static class SwarmUtils
{
    public static ServiceSpec CreateSpec(Guid sessionId, 
        string configId, 
        string serverPort, 
        string httpPort)
    {
        string resultPath = $"/data/zick-gs/results/session-{sessionId}";
        
        
        //TODO: Conf version not needed, Env variables for session.
        if (!Directory.Exists(resultPath))
            Directory.CreateDirectory(resultPath);
        
        return new()
        {
            Name = $"session-{sessionId}",
            TaskTemplate = new()
            {
                RestartPolicy = new()
                {
                    Delay = 3000,
                    Condition = "none",
                    MaxAttempts = 0
                },
                ContainerSpec = new()
                {
                    Image = "zickbalazs/assetto-server:latest",
                    Mounts = new List<Mount>()
                    {
                      new Mount()
                      {
                          Type = "bind",
                          Target = "/opt/app/assetto-dedicated/server/results",
                          Source = resultPath,
                      }
                    },
                    Command = new List<string>()
                    {
                        "/bin/sh",
                        "-c",
                        "echo swarm-full-started; sleep 30 ; cat -n /opt/app/assetto-dedicated/server/cfg/entry_list.ini ; /bin/sh /opt/app/assetto-dedicated/entrypoint.sh"
                    },
                    Configs = new List<SwarmConfigReference>()
                    {
                        new()
                        {
                            ConfigID = configId,
                            ConfigName = $"{sessionId}",
                            File = new()
                            {
                                Name = "/opt/app/assetto-dedicated/server/cfg/entry_list.ini",
                                UID = "0",
                                GID = "0",
                                Mode = 292
                            },
                        }
                    }
                }
            },
            EndpointSpec = CreateEndpoint(serverPort, httpPort),
            Mode = new ServiceMode()
            {
                Replicated = new ReplicatedService()
                {
                    Replicas = 1
                }
            }
        };
    }

    private static EndpointSpec CreateEndpoint(string serverPort, string httpPort)
    {
        return new()
        {
            Ports = new List<PortConfig>()
            {
                new()
                {
                    Protocol = "tcp",
                    PublishedPort = uint.Parse(serverPort),
                    TargetPort = uint.Parse(serverPort),
                    PublishMode = "host"
                },
                new()
                {
                    Protocol = "udp",
                    PublishedPort = uint.Parse(serverPort),
                    TargetPort = uint.Parse(serverPort),
                    PublishMode = "host"
                },
                new()
                {
                    Protocol = "tcp",
                    PublishedPort = uint.Parse(httpPort),
                    TargetPort = uint.Parse(httpPort),
                    PublishMode = "host"
                }
            }
        };
    }
}