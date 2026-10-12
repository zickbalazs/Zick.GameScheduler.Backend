using Microsoft.EntityFrameworkCore;
using Zick.GameScheduler.Backend.Data;
using Zick.GameScheduler.Backend.Data.DTOs;
using Zick.GameScheduler.Backend.Data.Models;
using Zick.GameScheduler.Services.ServicesAPI;

namespace Zick.GameScheduler.Services.API;

public class DbLeagueService(ApplicationContext<RacingUserIdentity> ctx) : ILeagueService
{
    public async Task<IList<LeagueDTO>> GetLeagues()
    {
        var leagues = await ctx.Leagues
            .Include(x=>x.Sessions)
            .ThenInclude(m=>m.Registrations)
            .ThenInclude(n=>n.User)
            .Include(x=>x.Sessions)
            .ThenInclude(x=>x.PortClaim)
            .Include(m=>m.CurrentTrack)
            .Include(x=>x.Classes)
            .ToListAsync();
        return [.. leagues];
    }

    public Task<LeagueDTO> GetLeague(Guid id)
    {
        throw new NotImplementedException();
    }
}