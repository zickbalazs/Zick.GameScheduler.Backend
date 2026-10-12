using Zick.GameScheduler.Backend.Data.Dashboard.Forms;
using Zick.GameScheduler.Backend.Data.DTOs;

namespace Zick.GameScheduler.Services.Data;

public class DbLeagueService : ILeagueService
{
    public Task AddLeague(AddLeagueForm form)
    {
        throw new NotImplementedException();
    }

    public Task DeleteLeague(Guid id)
    {
        throw new NotImplementedException();
    }

    public Task ModifyLeague(EditLeagueForm form)
    {
        throw new NotImplementedException();
    }

    public Task<IList<LeagueDTO>> GetLeagues()
    {
        throw new NotImplementedException();
    }

    public Task<LeagueDTO> GetLeagueById(Guid id)
    {
        throw new NotImplementedException();
    }

    public Task ChangeActiveTrack(Guid id, TrackDTO track)
    {
        throw new NotImplementedException();
    }

    public Task RemoveSessions(Guid id)
    {
        throw new NotImplementedException();
    }

    public Task AddSessions(Guid id)
    {
        throw new NotImplementedException();
    }
}