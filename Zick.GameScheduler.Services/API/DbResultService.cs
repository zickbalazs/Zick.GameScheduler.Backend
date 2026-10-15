using Zick.GameScheduler.Backend.Data.DTOs;

namespace Zick.GameScheduler.Services.ServicesAPI;

public class DbResultService : IResultService
{
    public Task<ResultDTO> GetResultForSession(Guid sessionId)
    {
        throw new NotImplementedException();
    }
}