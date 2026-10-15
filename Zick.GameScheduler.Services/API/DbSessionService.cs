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
        var session = await ctx.Sessions.FirstAsync(x => x.Id == id);
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
        throw new NotImplementedException();
    }

    public async Task RegisterToSession(Guid sessionId, string userId)
    {
        throw new NotImplementedException();
    }

    public async Task UnregisterFromSession(Guid sessionId, string userId)
    {
        throw new NotImplementedException();
    }

    /// <exception cref="ArgumentNullException">When <c>null</c> is given as any of the parameters</exception>
    /// <exception cref="InvalidOperationException">When either the registration, or the car was not found</exception>
    /// <exception cref="DbUpdateException">When saving to the database didn't happen</exception>
    /// <exception cref="DbUpdateConcurrencyException">When the locks are stuck, no changes happen</exception>
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