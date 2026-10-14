using Microsoft.EntityFrameworkCore;
using Zick.GameScheduler.Backend.Data;
using Zick.GameScheduler.Backend.Data.Dashboard.Forms;
using Zick.GameScheduler.Backend.Data.DTOs;
using Zick.GameScheduler.Backend.Data.Models;

namespace Zick.GameScheduler.Services.Data;

public class DbLeagueService(IDbContextFactory<ApplicationContext<RacingUserIdentity>> contextFactory) : ILeagueService
{
    public Task AddLeague(AddLeagueForm form, bool fillSessions)
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

    public async Task<IList<LeagueDTO>> GetLeagues()
    {
        await using (var ctx = await contextFactory.CreateDbContextAsync())
        {
            return
            [
                .. ctx.Leagues
                    .Include(x => x.Sessions)
                    .ThenInclude(x => x.Registrations)
                    .Include(x => x.CurrentTrack)
                    .Include(x => x.Classes)
            ];

        }
    }

    public async Task<LeagueDTO> GetLeagueById(Guid id)
    {
        await using (var ctx = await contextFactory.CreateDbContextAsync())
        {
            return await ctx.Leagues
                .Include(x => x.Sessions)
                .ThenInclude(x => x.Registrations)
                .Include(x => x.CurrentTrack)
                .Include(x => x.Classes)
                .FirstAsync(x => x.Id == id);
        }
    }

    public async Task ChangeActiveTrack(Guid id, TrackDTO track)
    {
        await using (var ctx = await contextFactory.CreateDbContextAsync())
        {
            var dbEntry = await ctx.Leagues
                .Include(x=>x.CurrentTrack)
                .FirstAsync(x => x.Id == id);

            dbEntry.CurrentTrack = await ctx.Tracks.FirstAsync(x => x.Id == track.Id);

            await ctx.SaveChangesAsync();
        }
    }

    public Task RemoveSessions(Guid id)
    {
        throw new NotImplementedException();
    }

    public Task AddSessions(Guid id, bool destructive)
    {
        throw new NotImplementedException();
    }
}