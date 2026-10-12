using Zick.GameScheduler.Backend.Data.DTOs;

namespace Zick.GameScheduler.Services.ServicesAPI;

public interface ITrackService
{
    Task<IList<TrackDTO>> GetTracks();
    Task<TrackDTO> GetTrack(Guid id);
}