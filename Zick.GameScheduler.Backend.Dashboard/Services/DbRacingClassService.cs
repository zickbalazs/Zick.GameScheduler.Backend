using Microsoft.EntityFrameworkCore;
using Zick.GameScheduler.Backend.Dashboard.Models.Form;
using Zick.GameScheduler.Backend.Data;
using Zick.GameScheduler.Backend.Data.DTOs;
using Zick.GameScheduler.Backend.Data.Models;

namespace Zick.GameScheduler.Backend.Dashboard.Services;

public class DbRacingClassService(ApplicationContext<RacingUserIdentity> ctx) : IRacingClassService
{
    public Task<(bool Success, string Reason)> DeleteClass(string id)
    {
        throw new NotImplementedException();
    }

    public Task<(bool Success, string Reason)> ModifyClass(EditRacingClassForm form)
    {
        throw new NotImplementedException();
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
        return await ctx.Classes.FirstAsync(x => x.Abbreviation == id);
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