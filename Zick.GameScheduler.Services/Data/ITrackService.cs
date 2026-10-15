using Zick.GameScheduler.Backend.Data.Dashboard.Forms;
using Zick.GameScheduler.Backend.Data.DTOs;

namespace Zick.GameScheduler.Services.Data;

public interface ITrackService
{
    Task AddTrack(AddTrackForm form);
    Task EditTrack(EditTrackForm form);
    Task DeleteTrack(Guid id);
    Task<IList<TrackDTO>> GetTracks();
    Task<TrackDTO> GetTrack(Guid id);
}