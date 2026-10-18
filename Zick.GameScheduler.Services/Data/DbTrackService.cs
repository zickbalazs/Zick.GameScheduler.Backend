using System.Data;
using Microsoft.EntityFrameworkCore;
using Zick.GameScheduler.Backend.Data;
using Zick.GameScheduler.Backend.Data.Dashboard.Forms;
using Zick.GameScheduler.Backend.Data.DTOs;
using Zick.GameScheduler.Backend.Data.Models;

namespace Zick.GameScheduler.Services.Data;

public class DbTrackService(IDbContextFactory<ApplicationContext<RacingUserIdentity>> ctxFactory) : ITrackService
{
    public async Task AddTrack(AddTrackForm form)
    {
        Track<RacingUserIdentity> trackEntry = new()
        {
            FolderName = form.Folder,
            Name = form.Name,
            CountryCode = form.CountryCode,
            MaxCapacity = form.MaxParticipants
        };

        await using (var ctx = await ctxFactory.CreateDbContextAsync())
        {
            var trackWithFolder = ctx.Tracks.Any(x => x.FolderName == form.Folder);
            if (trackWithFolder)
                throw new DuplicateNameException("Track with this folder name already exists");

            ctx.Tracks.Add(trackEntry);
            await ctx.SaveChangesAsync();
        }
    }

    public async Task EditTrack(EditTrackForm form)
    {
        await using (var ctx = await ctxFactory.CreateDbContextAsync())
        {
            if (ctx.Tracks.Where(x => x.Id != form.Id).Any(x => x.FolderName == form.Folder))
                throw new DuplicateNameException("Folder name must be unique for the track");

            var trackEntry = ctx.Tracks.First(x => x.Id == form.Id);

            trackEntry.FolderName = form.Folder;
            trackEntry.Name = form.Name;
            trackEntry.CountryCode = form.CountryCode;
            trackEntry.MaxCapacity = form.MaxParticipants;

            await ctx.SaveChangesAsync();
        }
    }

    public async Task DeleteTrack(Guid id)
    {
        await using (var ctx = await ctxFactory.CreateDbContextAsync())
        {
            if (ctx.Tracks.Count() < 2)
                throw new NotSupportedException("There must be at least one more track to delete this one.");

            var trackEntry = ctx.Tracks.First(x => x.Id == id);
            var firstTrack = ctx.Tracks.First(x => x.Id != id);

            var leaguesUsingTrack = await ctx.Leagues
                .Include(m => m.CurrentTrack)
                .Where(x => x.CurrentTrack.Id == id)
                .ToListAsync();

            foreach (var track in leaguesUsingTrack)
            {
                track.CurrentTrack = firstTrack;
                firstTrack.CurrentEvents.Add(track);
            }
            ctx.Tracks.Remove(trackEntry);
            await ctx.SaveChangesAsync();
        }
    }

    public async Task<IList<TrackDTO>> GetTracks()
    {
        await using (var ctx = await ctxFactory.CreateDbContextAsync())
        {
            return [ ..ctx.Tracks ];
        }
    }

    public async Task<TrackDTO> GetTrack(Guid id)
    {
        await using (var ctx = await ctxFactory.CreateDbContextAsync())
        {
            return await ctx.Tracks.FirstAsync(x => x.Id == id);
        }
    }
}