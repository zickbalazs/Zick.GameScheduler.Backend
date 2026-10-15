using System.Data;
using Microsoft.EntityFrameworkCore;
using Zick.GameScheduler.Backend.Data;
using Zick.GameScheduler.Backend.Data.Dashboard.Forms;
using Zick.GameScheduler.Backend.Data.DTOs;
using Zick.GameScheduler.Backend.Data.Models;

namespace Zick.GameScheduler.Services.Data;

public class DbVehicleService(IDbContextFactory<ApplicationContext<RacingUserIdentity>> ctxFactory) : IVehicleService
{
    public async Task AddVehicle(AddVehicleForm form)
    {
        await using (var ctx = await ctxFactory.CreateDbContextAsync())
        {
            bool hasSameCars = ctx.Cars.Any(x => x.FolderName == form.FolderName);
            
            if (hasSameCars)
                throw new DuplicateNameException("Car with this folder name already exists!");
            
            ctx.Cars.Add(new Car<RacingUserIdentity>()
            {
                Manufacturer = form.Manufacturer,
                Model = form.Model,
                FolderName = form.FolderName
            });
            
            await ctx.SaveChangesAsync();
        }
    }

    public async Task EditVehicle(EditVehicleForm form)
    {
        await using (var ctx = await ctxFactory.CreateDbContextAsync())
        {
            var entry = await ctx.Cars.FirstAsync(x => x.Id == form.Id);

            bool sameEntry = ctx.Cars
                .Where(x => x.Id != entry.Id)
                .Any(m => m.FolderName == form.Folder);

            if (sameEntry)
                throw new DuplicateNameException("The updated folder name is a duplicate with another car!");
            
            entry.Manufacturer = form.Manufacturer;
            entry.Model = form.Model;
            entry.FolderName = form.Folder;

            await ctx.SaveChangesAsync();
        }
    }

    public async Task DeleteVehicle(Guid id)
    {
        await using (var ctx = await ctxFactory.CreateDbContextAsync())
        {
            var entry = await ctx.Cars.FirstAsync(x => x.Id == id);
            ctx.Remove(entry);
            await ctx.SaveChangesAsync();
        }
    }

    public async Task<IList<CarDTO>> GetVehicles()
    {
        await using (var ctx = await ctxFactory.CreateDbContextAsync())
        {
            return [.. ctx.Cars];
        }
    }

    public async Task<CarDTO> GetVehicle(Guid id)
    {
        await using (var ctx = await ctxFactory.CreateDbContextAsync())
        {
            var car = await ctx.Cars.FirstAsync(x => x.Id == id);
            return car;
        }
    }
}