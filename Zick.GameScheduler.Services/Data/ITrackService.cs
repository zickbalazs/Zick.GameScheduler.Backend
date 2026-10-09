using Zick.GameScheduler.Backend.Data.Dashboard.Forms;
using Zick.GameScheduler.Backend.Data.DTOs;

namespace Zick.GameScheduler.Services.Data;

public interface ITrackService
{
    Task<bool> AddTrack(AddTrackForm form);
    Task<(bool Success, string Message)> EditTrack(EditTrackForm form);
    Task<(bool Success, string Message)> DeleteTrack(Guid id);
    Task<IList<TrackDTO>> GetTracks();
    Task<TrackDTO> GetTrack(Guid id);
}