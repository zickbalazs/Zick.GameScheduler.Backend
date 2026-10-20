using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Zick.GameScheduler.Backend.Data;
using Zick.GameScheduler.Backend.Data.DTOs;
using Zick.GameScheduler.Backend.Data.Models;

namespace Zick.GameScheduler.Services.API;

public class DbRegistrationService(ApplicationContext<RacingUserIdentity> ctx,
    ILogger<DbRegistrationService> logger) : IRegistrationService
{
    public async Task<IList<RegistrationDTO>> GetRegistrations(string userId)
    {
      var regs = await ctx.Registrations
            .Include(x => x.Car)
            .Include(x => x.Session)
            .Include(x => x.User)
            .Where(x => x.User.Id == userId)
            .Where(x=>(x.Session.Status == SessionStatus.Waiting || x.Session.Status == SessionStatus.Starting || x.Session.Status == SessionStatus.Underway))
            .ToListAsync();
      return [.. regs];
    }

    public async Task ModifyRegistration(Guid registrationId, Guid newCarId)
    {
        var registration = await ctx.Registrations
            .Include(x => x.Session)
            .Include(x => x.Car)
            .FirstAsync(x => x.Id == registrationId);

        var car = await ctx.Cars.FirstAsync(x => x.Id == newCarId);
        
        if (registration.Session.Status != SessionStatus.Waiting)
            throw new InvalidOperationException(
                "The registration could not be modified, because the wait time is over.");

        registration.Car = car;
        logger.LogInformation("user changed car to {carId} for session: {sessionId}",
            car.Id,
            registration.Session.Id);
        await ctx.SaveChangesAsync();

    }

    public async Task CancelRegistration(Guid registrationId, string userId)
    {
        var registration = await ctx.Registrations
            .Include(x => x.Session)
            .Include(x=>x.User)
            .FirstAsync(x => x.Id == registrationId);

        if (registration.User.Id != userId)
            throw new UnauthorizedAccessException("You are not allowed to delete other user's registrations!");
        if (registration.Session.Status != SessionStatus.Waiting)
            throw new InvalidOperationException(
                "The registration could not be cancelled, because the wait time is over.");

        ctx.Remove(registration);
        logger.LogInformation("user cancelled registration from session id: {sessionId}", registration.Session.Id);
        await ctx.SaveChangesAsync();
    }
}