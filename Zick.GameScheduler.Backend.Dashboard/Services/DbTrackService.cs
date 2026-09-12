using Microsoft.EntityFrameworkCore;
using Zick.GameScheduler.Backend.Dashboard.Models.Form;
using Zick.GameScheduler.Backend.Data;
using Zick.GameScheduler.Backend.Data.DTOs;
using Zick.GameScheduler.Backend.Data.Models;

namespace Zick.GameScheduler.Backend.Dashboard.Services;

public class DbTrackService(ApplicationContext<RacingUserIdentity> ctx, ILogger<DbTrackService> logger) : ITrackService
{
    public async Task<bool> AddTrack(AddTrackForm form)
    {
        logger.LogTrace("starting track addition into database");
        try
        {
            Track<RacingUserIdentity> addition = new()
            {
                Name = form.Name,
                FolderName = form.Folder,
                CountryCode = form.CountryCode,
            };

            await ctx.AddAsync(addition);
            int affectedRows = await ctx.SaveChangesAsync();
            bool success = affectedRows > 0;
            if (success)
                logger.LogInformation("added new entry to tracks with id: {}", addition.Id);
            else
                logger.LogError("addition was not successful into db, no rows were changed");
            return success;
        }
        catch (Exception e)
        {
            logger.LogError(e.Message);
            return false;
        }
    }

    public Task<(bool Success, string Message)> EditTrack(EditTrackForm form)
    {
        throw new NotImplementedException();
    }

    public async Task<(bool Success, string Message)> DeleteTrack(Guid id)
    {
        try
        {
            var leagueCountForTrack = ctx.Leagues
                .Include(x => x.CurrentTrack)
                .Count(x => x.CurrentTrack.Id == id);

            if (leagueCountForTrack > 0)
                return (false, "Track is used in at least one league currently!");

            var sessionCountForTrack = ctx.Sessions
                .Include(m => m.Track)
                .Count(x => x.Track != null && x.Track.Id == id);

            if (sessionCountForTrack > 0)
                return (false, "Track is used in at least one session!");

            var dbEntry = await ctx.Tracks.FirstAsync(x => x.Id == id);
            ctx.Tracks.Remove(dbEntry);
            
            var affectedRows = await ctx.SaveChangesAsync();
            
            if (affectedRows > 0)
                logger.LogInformation("deleted track with id: {id}", id);
            else 
                logger.LogError("track deletion was not possible, rows were not affected");
            
            return (affectedRows > 0, affectedRows > 0 ? "" : "Nothing was deleted!");
        }
        catch (Exception e)
        {
            logger.LogError("deletion of track with id: {id} failed due to error: {errorMessage}", id, e.Message);
            return (false, e.Message);
        }
    }

    public async Task<IList<TrackDTO>> GetTracks()
    {
        return await ctx.Tracks.Select(m=>(TrackDTO)m).ToListAsync();
    }

    public async Task<TrackDTO> GetTrack(Guid id)
    {
        return await ctx.Tracks.FirstAsync(x=>x.Id==id);
    }
}