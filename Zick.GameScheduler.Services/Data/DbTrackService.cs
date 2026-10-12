using Zick.GameScheduler.Backend.Data.Dashboard.Forms;
using Zick.GameScheduler.Backend.Data.DTOs;

namespace Zick.GameScheduler.Services.Data;

public class DbTrackService : ITrackService
{
    public Task AddTrack(AddTrackForm form)
    {
        throw new NotImplementedException();
    }

    public Task EditTrack(EditTrackForm form)
    {
        throw new NotImplementedException();
    }

    public Task DeleteTrack(Guid id)
    {
        throw new NotImplementedException();
    }

    public Task<IList<TrackDTO>> GetTracks()
    {
        throw new NotImplementedException();
    }

    public Task<TrackDTO> GetTrack(Guid id)
    {
        throw new NotImplementedException();
    }
}