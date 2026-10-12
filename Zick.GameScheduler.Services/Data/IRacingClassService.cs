using Zick.GameScheduler.Backend.Data.Dashboard.Forms;
using Zick.GameScheduler.Backend.Data.DTOs;

namespace Zick.GameScheduler.Services.Data;

public interface IRacingClassService
{
    Task DeleteClass(string id);
    Task ModifyClass(EditRacingClassForm form);
    Task<IList<RacingClassDTO>> GetClasses();
    Task<RacingClassDTO> GetClass(string id);
    Task AddClass(AddRacingClassForm form);
}