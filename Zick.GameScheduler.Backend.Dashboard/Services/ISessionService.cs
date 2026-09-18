using Zick.GameScheduler.Backend.Dashboard.Models.Form;
using Zick.GameScheduler.Backend.Data.DTOs;

namespace Zick.GameScheduler.Backend.Dashboard.Services;

public interface ISessionService
{
    Task<IList<SessionDTO>> GetCustomSessions();
    Task<IList<SessionDTO>> GetSessionByLeague(Guid leagueId);
    Task<SessionDTO> GetSession(Guid id);
    Task<(bool Success, string Reason)> AddSession(AddSessionForm form);
    Task<(bool Success, string Reason)> UpdateSession(EditSessionForm form);
    Task<(bool Success, string Reason)> DeleteSession(Guid id);
    Task<bool> CancelSession(Guid id);
    Task<bool> StartSession(Guid id);
}