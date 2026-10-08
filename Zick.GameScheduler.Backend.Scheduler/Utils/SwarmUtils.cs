using Docker.DotNet.Models;

namespace Zick.GameScheduler.Backend.Scheduler.Utils;

public static class SwarmUtils
{
    public static ServiceSpec CreateSpec(Guid sessionId, string configId)
    {
        return new()
        {
            Name = "assetto-server",
            TaskTemplate = new()
            {
                ContainerSpec = new()
                {
                    Image = "assetto-server:latest",
                    Configs = new List<SwarmConfigReference>()
                    {
                        new()
                        {
                            ConfigName = sessionId.ToString(),
                            ConfigID = configId,
                            File = new()
                            {
                                Name = "/opt/app/assetto-server/server/cfg/entry_list.ini"
                            }
                        }
                    }
                }
            },
            Mode = new ServiceMode()
            {
                Replicated = new ReplicatedService()
                {
                    Replicas = 1
                }
            }
        };
    }
}