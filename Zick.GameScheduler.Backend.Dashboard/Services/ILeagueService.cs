using Zick.GameScheduler.Backend.Data.DTOs;

namespace Zick.GameScheduler.Backend.Dashboard.Services;

public interface ILeagueService
{
    Task<(bool Success, string Reason)> AddLeague();
    Task<(bool Success, string Reason)> DeleteLeague();
    Task<(bool Success, string Reason)> ModifyLeague();
    Task<IList<LeagueDTO>> GetLeagues();
    Task<LeagueDTO> GetLeagueById();
}