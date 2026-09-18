using Zick.GameScheduler.Backend.Dashboard.Models.Form;
using Zick.GameScheduler.Backend.Data.DTOs;

namespace Zick.GameScheduler.Backend.Dashboard.Services;

public interface ITrackService
{
    Task<bool> AddTrack(AddTrackForm form);
    Task<(bool Success, string Message)> EditTrack(EditTrackForm form);
    Task<(bool Success, string Message)> DeleteTrack(Guid id);
    Task<IList<TrackDTO>> GetTracks();
    Task<TrackDTO> GetTrack(Guid id);
}