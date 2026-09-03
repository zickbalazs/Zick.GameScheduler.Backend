using Microsoft.EntityFrameworkCore;
using Zick.GameScheduler.Backend.Dashboard.Models.Form;
using Zick.GameScheduler.Backend.Data;
using Zick.GameScheduler.Backend.Data.DTOs;
using Zick.GameScheduler.Backend.Data.Models;

namespace Zick.GameScheduler.Backend.Dashboard.Services;

public class DbRacingClassService(ApplicationContext<RacingUserIdentity> ctx) : IRacingClassService
{
    public async Task<(bool Success, string Reason)> DeleteClass(string id)
    {
        try
        {
            var rClass = await ctx.Classes
                .Include(x => x.Leagues)
                .FirstAsync(m => m.Abbreviation == id);
            
            int classInLeagues = rClass.Leagues.Count;

            if (classInLeagues > 0) throw new Exception("This class is currently used in at least 1 league");

            ctx.Classes.Remove(rClass);
            int rowsAffected = await ctx.SaveChangesAsync();

            return (rowsAffected > 0, rowsAffected > 0 ? "" : "Nothing was deleted!");
        }
        catch (Exception e)
        {
            return (false, e.Message);
        }
    }

    public async Task<(bool Success, string Reason)> ModifyClass(EditRacingClassForm form)
    {
        try
        {
            var rClass = await ctx.Classes
                .Include(x => x.Cars)
                .FirstAsync(m => m.Abbreviation == form.Identifier);

            // Clean cars
            rClass.Cars.Clear();
            // Add cars that are set now
            var selectedCars = ctx.Cars.Where(x=>form.Cars.Select(b=>b.Id).Contains(x.Id));

            foreach (var car in selectedCars)
            {
                rClass.Cars.Add(car);
            }            
            
            rClass.Name = form.Name;
            rClass.Color = form.Color;
            
            int rowsAffected = await ctx.SaveChangesAsync();
            return (rowsAffected > 0, rowsAffected > 0 ? "" : "Nothing was updated!");
        }
        catch (Exception e)
        {
            return (false, e.Message);
        }
    }

    public async Task<IList<RacingClassDTO>> GetClasses()
    {
        return await ctx.Classes
            .Include(x => x.Cars)
            .Select(y => (RacingClassDTO)y)
            .ToListAsync();
    }

    public async Task<RacingClassDTO> GetClass(string id)
    {
        return await ctx.Classes
            .Include(m=>m.Cars)
            .FirstAsync(x => x.Abbreviation == id);
    }

    public async Task<(bool Success, string Reason)> AddClass(AddRacingClassForm form)
    {
        if (form.Cars.Count < 1)
        {
            return (false, "At least one car must be added for a racing class!");
        }
        try
        {
            var entry = new RacingClass<RacingUserIdentity>()
            {
                Abbreviation = form.Identifier,
                Color = form.Color,
                Name = form.Name,
            };

            var carDbEntries = ctx.Cars
                .Where(m => form.Cars.Select(h => h.Id)
                .Contains(m.Id));

            ctx.Add(entry);
            
            foreach (var car in carDbEntries)
            {
                entry.Cars.Add(car);
                car.Classes.Add(entry);
            }
            
            await ctx.SaveChangesAsync();
            return (true, "");
        }
        catch (Exception e)
        {
            return (false, e.Message);
        }
    }
}