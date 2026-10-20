using Microsoft.EntityFrameworkCore;
using Zick.GameScheduler.Backend.Data;
using Zick.GameScheduler.Backend.Data.DTOs;
using Zick.GameScheduler.Backend.Data.Models;

namespace Zick.GameScheduler.Services.ServicesAPI;

public class DbVehicleService(ApplicationContext<RacingUserIdentity> ctx) : IVehicleService
{
    public async Task<IList<CarDTO>> GetCarsByClass(string classId)
    {
        var raceClass = ctx.Classes
            .Include(m=>m.Cars)
            .First(x => x.Abbreviation == classId);

        return [.. raceClass.Cars];

    }

    public async Task<IList<CarDTO>> GetCars()
    {
        return [.. (await ctx.Cars.ToListAsync())];
    }

    public async Task<CarDTO> GetCar(Guid id)
    {
        return await ctx.Cars.FirstAsync(x => x.Id == id);
    }
}