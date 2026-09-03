using Microsoft.EntityFrameworkCore;
using MudBlazor;
using Zick.GameScheduler.Backend.Dashboard.Models.Form;
using Zick.GameScheduler.Backend.Data;
using Zick.GameScheduler.Backend.Data.DTOs;
using Zick.GameScheduler.Backend.Data.Models;

namespace Zick.GameScheduler.Backend.Dashboard.Services;

public class DbVehicleService(ApplicationContext<RacingUserIdentity> ctx) : IVehicleService
{
    public async Task<bool> AddVehicle(AddVehicleForm form)
    {
        Car<RacingUserIdentity> car = new()
        {
            FolderName = form.FolderName,
            Manufacturer = form.Manufacturer,
            Model = form.Model
        };

        ctx.Cars.Add(car);

        try
        {
            var s = await ctx.SaveChangesAsync();
            return s > 0;
        }
        catch (Exception)
        {
            return false;
        }
    }

    public async Task<(bool Success, string Reason)> EditVehicle(EditVehicleForm form)
    {
        try
        {
            var matches = ctx.Cars.Where(x => x.Id == form.Id);

            await matches.ExecuteUpdateAsync(m =>
            {
                m.SetProperty(x => x.Manufacturer, form.Manufacturer)
                    .SetProperty(x => x.Model, form.Model)
                    .SetProperty(x => x.FolderName, form.Folder);
            });

            var updatedRows = await ctx.SaveChangesAsync();
            return (updatedRows > 0, "");
        }
        catch (Exception e)
        {
            return (false, e.Message);
        }
    }

    public async Task<(bool Success, string Reason)> DeleteVehicle(Guid id)
    {
        try
        {
            var car = await ctx.Cars.FirstAsync(x => x.Id == id);
            ctx.Cars.Remove(car);
            await ctx.SaveChangesAsync();
            return (true, "");
        }
        catch (Exception e)
        {
            return (false, e.Message);
        }
    }

    public async Task<IList<CarDTO>> GetVehicles()
    {
        return await ctx.Cars.Select(x => (CarDTO)x).ToListAsync();
    }

    public async Task<CarDTO> GetVehicle(Guid id)
    {
        return await ctx.Cars.FirstAsync(x => x.Id == id);
    }
}