using Microsoft.EntityFrameworkCore;
using Zick.GameScheduler.Backend.Data;
using Zick.GameScheduler.Backend.Data.Dashboard.Forms;
using Zick.GameScheduler.Backend.Data.DTOs;
using Zick.GameScheduler.Backend.Data.Models;

namespace Zick.GameScheduler.Services.Data;

public class DbRacingClassService(IDbContextFactory<ApplicationContext<RacingUserIdentity>> ctxFactory) : IRacingClassService
{
    public async Task DeleteClass(string id)
    {
        await using (var ctx = await ctxFactory.CreateDbContextAsync())
        {
            var entry = await ctx.Classes
                .FirstAsync(x => x.Abbreviation == id);
            ctx.Classes.Remove(entry);
            await ctx.SaveChangesAsync();
        }
    }

    public Task ModifyClass(EditRacingClassForm form)
    {
        throw new NotImplementedException();
    }

    public async Task<IList<RacingClassDTO>> GetClasses()
    {
        await using (var ctx = await ctxFactory.CreateDbContextAsync())
        {
            var raceClasses = ctx.Classes
                .Include(x => x.Cars);
            return [ ..raceClasses ];
        }
    }

    public async Task<RacingClassDTO> GetClass(string id)
    {
        await using (var ctx = await ctxFactory.CreateDbContextAsync())
        {
            var raceClass = await ctx.Classes
                .Include(m=>m.Cars)
                .FirstAsync(x => x.Abbreviation == id);
            return raceClass;
        }
    }
    
    public async Task AddClass(AddRacingClassForm form)
    {
        var formCars = form.Cars.Select(x => new Car<RacingUserIdentity>()
        {
            Id = x.Id,
            Manufacturer = x.Manufacturer,
            Model = x.Model,
            FolderName = x.FolderName
        }).ToList();
        
        await using (var ctx = await ctxFactory.CreateDbContextAsync())
        {
            var newClass = ctx.Classes.Add(new()
            {
                Abbreviation = form.Identifier,
                Name = form.Name,
                Color = form.Color
            });
            formCars.ForEach(newClass.Entity.Cars.Add);
            await ctx.SaveChangesAsync();
        }
    }
}