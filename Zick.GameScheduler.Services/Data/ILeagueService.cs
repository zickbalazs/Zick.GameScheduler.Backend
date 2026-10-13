using Zick.GameScheduler.Backend.Data.Dashboard.Forms;
using Zick.GameScheduler.Backend.Data.DTOs;

namespace Zick.GameScheduler.Services.Data;

public interface ILeagueService
{
    Task AddLeague(AddLeagueForm form);
    Task DeleteLeague(Guid id);
    Task ModifyLeague(EditLeagueForm form);
    Task<IList<LeagueDTO>> GetLeagues();
    Task<LeagueDTO> GetLeagueById(Guid id);
    Task ChangeActiveTrack(Guid id, TrackDTO track);
    Task RemoveSessions(Guid id);
    Task AddSessions(Guid id, bool destructive);
}