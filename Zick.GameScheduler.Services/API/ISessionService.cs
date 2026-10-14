using Zick.GameScheduler.Backend.Data.DTOs;

namespace Zick.GameScheduler.Services.ServicesAPI;

public interface ISessionService
{
    Task<SessionDTO> GetSession(Guid id);
    Task<IList<SessionDTO>> GetSessionsByLeagueId(Guid leagueId);
    Task<IList<SessionDTO>> GetRegisteredSessions(string userId);
    Task<IList<SessionDTO>> GetAllSessions();
    Task<IList<SessionDTO>> GetCustomSessions();
    Task RegisterToSession(Guid sessionId, string userId, Guid vehicleId);
    Task UnregisterFromSession(Guid sessionId, string userId);
    Task AlterRegistrationToSession(Guid sessionId, string userId, Guid newVehicleId);
}