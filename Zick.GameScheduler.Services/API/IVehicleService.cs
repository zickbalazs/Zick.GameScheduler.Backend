using Zick.GameScheduler.Backend.Data.DTOs;

namespace Zick.GameScheduler.Services.ServicesAPI;

public interface IVehicleService
{
    Task<IList<CarDTO>> GetCarsByClass(string classId);
    Task<IList<CarDTO>> GetCars();
}