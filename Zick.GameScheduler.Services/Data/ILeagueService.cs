using Zick.GameScheduler.Backend.Data.Dashboard.Forms;
using Zick.GameScheduler.Backend.Data.DTOs;

namespace Zick.GameScheduler.Services.Data;

public interface ILeagueService
{
    Task<(bool Success, string Reason)> AddLeague(AddLeagueForm form);
    Task<(bool Success, string Reason)> DeleteLeague(Guid id);
    Task<(bool Success, string Reason)> ModifyLeague(EditLeagueForm form);
    Task<IList<LeagueDTO>> GetLeagues();
    Task<LeagueDTO> GetLeagueById(Guid id);
    Task<(bool Success, string Reason)> ChangeActiveTrack(Guid id, TrackDTO track);
    Task<(bool Success, string Reason)> RemoveSessions(Guid id);
    Task<(bool Success, string Reason)> AddSessions(Guid id);
}