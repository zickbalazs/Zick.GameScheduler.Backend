using Microsoft.EntityFrameworkCore;
using Zick.GameScheduler.Backend.Dashboard.Models.Form;
using Zick.GameScheduler.Backend.Data;
using Zick.GameScheduler.Backend.Data.DTOs;
using Zick.GameScheduler.Backend.Data.Models;

namespace Zick.GameScheduler.Backend.Dashboard.Services;

public class DbLeagueService(ApplicationContext<RacingUserIdentity> ctx, ILogger<DbLeagueService> logger) : ILeagueService
{
    public Task<(bool Success, string Reason)> AddLeague(AddLeagueForm form)
    {
        throw new NotImplementedException();
    }

    public async Task<(bool Success, string Reason)> DeleteLeague(Guid id)
    {
        try
        {
            var league = await ctx.Leagues.FirstAsync(x => x.Id == id);
            ctx.Leagues.Remove(league);
            var affectedRows = await ctx.SaveChangesAsync();

            // TODO: Logging here
            
            
            return (affectedRows > 0, "");
        }
        catch (Exception e)
        {
            logger.LogError("track deletion failed with error: {errorMessage}", e.Message);
            return (false, e.Message);
        }
    }

    public Task<(bool Success, string Reason)> ModifyLeague(EditLeagueForm form)
    {
        throw new NotImplementedException();
    }

    public async Task<IList<LeagueDTO>> GetLeagues()
    {
        var list = await ctx.Leagues
            .Include(x => x.CurrentTrack)
            .Include(x => x.Classes)
            .ToListAsync();
        return [.. list];
    }

    public async Task<LeagueDTO> GetLeagueById(Guid id)
    {
        return await ctx.Leagues
            .Include(x => x.CurrentTrack)
            .Include(x => x.Classes)
            .FirstAsync(x => x.Id == id);
    }

    public async Task<(bool Success, string Reason)> ChangeActiveTrack(Guid id, TrackDTO track)
    {
        try
        {
            var dbTrack = await ctx.Tracks.FirstAsync(x => x.Id == track.Id);

            var league = await ctx.Leagues.FirstAsync(x => x.Id == id);

            league.CurrentTrack = dbTrack;
            var affectedRows = await ctx.SaveChangesAsync();
            if (affectedRows > 0)
                logger.LogInformation(
                    "changed active track for league entry: {leagueId}, current active track is with id: {trackId}", id,
                    track.Id);
            else
                logger.LogError("Active track was not changed for track with id: {trackId}", track.Id);
            return (affectedRows > 0, "");
        }
        catch (Exception e)
        {
            logger.LogError("league active track addition failed with error: {errorMessage}", e.Message);
            return (false, e.Message);
        }
    }

    public async Task<(bool Success, string Reason)> RemoveSessions(Guid id)
    {
        try
        {
            var league = await ctx.Leagues
                .Include(m => m.Sessions)
                .FirstAsync(m => m.Id == id);

            league.Sessions.Where(SessionNotInUse).ToList().ForEach(x => { ctx.Sessions.Remove(x); });

            var affectedRows = await ctx.SaveChangesAsync();
            
            
            
            return (affectedRows>0, "");
        }
        catch (Exception e)
        {
            logger.LogError("Session pruning failed with error: {errorMessage}", e.Message);
            return (false, e.Message);
        }
    }

    private bool SessionNotInUse(LeagueSession<RacingUserIdentity> session) =>
        session.Status != SessionStatus.Starting && session.Status != SessionStatus.Underway &&
        session.Status != SessionStatus.Finished;
    
    
    
    public Task<(bool Success, string Reason)> AddSessions(Guid id)
    {
        throw new NotImplementedException();
    }
}