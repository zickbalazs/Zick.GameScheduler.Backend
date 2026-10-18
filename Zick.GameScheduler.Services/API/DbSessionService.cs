using System.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Zick.GameScheduler.Backend.Data;
using Zick.GameScheduler.Backend.Data.DTOs;
using Zick.GameScheduler.Backend.Data.Models;

namespace Zick.GameScheduler.Services.ServicesAPI;

public class DbSessionService(ApplicationContext<RacingUserIdentity> ctx) : ISessionService
{
    public async Task<SessionDTO> GetSession(Guid id)
    {
        var session = await ctx.Sessions
            .Include(x=>x.Registrations)
            .FirstAsync(x => x.Id == id);
        return session;
    }

    public async Task<IList<SessionDTO>> GetSessionsByLeagueId(Guid leagueId)
    {
        var league = await ctx.Leagues
            .Include(m=>m.Sessions)
            .ThenInclude(m=>m.Registrations)
            .FirstAsync(x => x.Id == leagueId);

        return [ ..league.Sessions ];
    }

    public async Task<IList<SessionDTO>> GetRegisteredSessions(string userId)
    {
        var sessionsByUser = await ctx.Sessions
            .Include(m => m.Registrations)
            .ThenInclude(m=>m.User)
            .Include(x=>x.Registrations)
            .ThenInclude(m=>m.Car)
            .Where(x=> x.Registrations.Any(m=>m.User.Id == userId))
            .ToListAsync();

        return [ ..sessionsByUser ];
    }

    public async Task<IList<SessionDTO>> GetAllSessions()
    {
        var sessions = ctx.Sessions
            .Include(x => x.Registrations)
            .ThenInclude(m => m.User);
        
        return [ ..sessions ];
    }

    public async Task<IList<SessionDTO>> GetCustomSessions()
    {
        var sessions = await ctx.Sessions
            .Include(x => x.League)
            .Include(x => x.Registrations)
            .ThenInclude(x => x.User)
            .Where(x => x.League.Name == "Custom Racing League")
            .ToListAsync();

        return [ ..sessions ];
    }

    public async Task RegisterToSession(Guid sessionId, string userId, Guid carId)
    {
        var session = ctx.Sessions
                .Include(x=>x.Registrations)
                .ThenInclude(m=>m.User)
                .Include(x=>x.League)
                .ThenInclude(m=>m.Classes)
                .ThenInclude(m=>m.Cars)
                .First(x => x.Id == sessionId);

        if (session.Status != SessionStatus.Waiting)
            throw new MethodAccessException("Registration to this session is not allowed.");

        bool carAllowed = session.League.Classes
            .Any(x => x.Cars.Any(m => m.Id == carId));

        if (!carAllowed)
            throw new UnauthorizedAccessException("Registration to the session with this car is not allowed!");

        if (session.Registrations.Any(x => x.User.Id == userId))
            throw new DuplicateNameException("Registration to this session was already done");

        ctx.Registrations.Add(new()
        {
            User = ctx.Users.First(x=>x.Id == userId),
            Session = session,
            Car = ctx.Cars.First(x=>x.Id == carId)
        });
        await ctx.SaveChangesAsync();
    }

    public async Task UnregisterFromSession(Guid sessionId, string userId)
    {
        throw new NotImplementedException();
    }
    
    public async Task AlterRegistrationToSession(Guid sessionId, string userId, Guid newVehicleId)
    {
        var registration = await ctx.Registrations
            .Include(m => m.User)
            .Include(m => m.Car)
            .Include(m => m.Session)
            .FirstAsync(x=>x.User.Id == userId && x.Session.Id == sessionId);

        var car = await ctx.Cars.FirstAsync(x => x.Id == newVehicleId);

        registration.Car = car;

        await ctx.SaveChangesAsync();
    }
    
}