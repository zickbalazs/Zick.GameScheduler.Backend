using Zick.GameScheduler.Backend.Dashboard.Models.Form;
using Zick.GameScheduler.Backend.Data.DTOs;

namespace Zick.GameScheduler.Backend.Dashboard.Services;

public interface IVehicleService
{
    Task<bool> AddVehicle(AddVehicleForm form);
    Task<(bool Success, string Reason)> EditVehicle(EditVehicleForm form);
    Task<(bool Success, string Reason)> DeleteVehicle(Guid id);
    Task<IList<CarDTO>> GetVehicles();
    Task<CarDTO> GetVehicle(Guid id);
}