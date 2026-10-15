using Microsoft.EntityFrameworkCore;
using Zick.GameScheduler.Backend.Data;
using Zick.GameScheduler.Backend.Data.DTOs;
using Zick.GameScheduler.Backend.Data.Models;

namespace Zick.GameScheduler.Services.ServicesAPI;

public class DbTrackService(ApplicationContext<RacingUserIdentity> ctx) : ITrackService
{
    public async Task<IList<TrackDTO>> GetTracks()
    {
        var tracks = await ctx.Tracks.ToListAsync();
        return [.. tracks];
    }

    public async Task<TrackDTO> GetTrack(Guid id)
    {
        return await ctx.Tracks.FirstAsync(x => x.Id == id);
    }
}