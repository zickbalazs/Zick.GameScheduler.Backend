using System.Data;
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

    public async Task ModifyClass(EditRacingClassForm form)
    {
        await using (var ctx = await ctxFactory.CreateDbContextAsync())
        {
            var entry = ctx.Classes
                .Include(x => x.Cars)
                .First(x => x.Abbreviation == form.Identifier);

            entry.Name = form.Name;
            entry.Color = form.Color;

            entry.Cars.Clear();
            
            foreach (var car in form.Cars)
            {
                entry.Cars.Add(ctx.Cars.First(x=>x.Id == car.Id));
            }

            await ctx.SaveChangesAsync();
        }
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
        await using (var ctx = await ctxFactory.CreateDbContextAsync())
        {
            if (ctx.Classes.Any(x => x.Abbreviation == form.Identifier))
                throw new DuplicateNameException("The class' identifier must be unique!");
            
            var newClass = ctx.Classes.Add(new()
            {
                Abbreviation = form.Identifier,
                Name = form.Name,
                Color = form.Color
            });

            foreach (var car in form.Cars)
            {
                newClass.Entity.Cars.Add(ctx.Cars.First(x=>x.Id == car.Id));
            }
            
            await ctx.SaveChangesAsync();
        }
    }
}