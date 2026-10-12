using Zick.GameScheduler.Backend.Data.DTOs;

namespace Zick.GameScheduler.Services.ServicesAPI;

public interface ILeagueService
{
    Task<IList<LeagueDTO>> GetLeagues();
    Task<LeagueDTO> GetLeague(Guid id);
}