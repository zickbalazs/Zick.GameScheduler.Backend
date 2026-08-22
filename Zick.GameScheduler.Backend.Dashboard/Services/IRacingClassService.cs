using Zick.GameScheduler.Backend.Dashboard.Models.Form;
using Zick.GameScheduler.Backend.Data.DTOs;

namespace Zick.GameScheduler.Backend.Dashboard.Services;

public interface IRacingClassService
{
    Task<(bool Success, string Reason)> DeleteClass(string id);
    Task<(bool Success, string Reason)> ModifyClass(EditRacingClassForm form);
    Task<IList<RacingClassDTO>> GetClasses();
    Task<RacingClassDTO> GetClass(string id);
    Task<(bool Success, string Reason)> AddClass(AddRacingClassForm form);
}