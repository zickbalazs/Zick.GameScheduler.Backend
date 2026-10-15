using Zick.GameScheduler.Backend.Data.Dashboard.Forms;
using Zick.GameScheduler.Backend.Data.DTOs;

namespace Zick.GameScheduler.Services.Data;

public interface IVehicleService
{
    Task AddVehicle(AddVehicleForm form);
    Task EditVehicle(EditVehicleForm form);
    Task DeleteVehicle(Guid id);
    Task<IList<CarDTO>> GetVehicles();
    Task<CarDTO> GetVehicle(Guid id);
}