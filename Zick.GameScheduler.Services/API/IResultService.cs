using Zick.GameScheduler.Backend.Data.DTOs;

namespace Zick.GameScheduler.Services.ServicesAPI;

public interface IResultService
{
    Task<ResultDTO> GetResultForSession(Guid sessionId);
}