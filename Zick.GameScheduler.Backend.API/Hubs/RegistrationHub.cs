using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Zick.GameScheduler.Backend.Data;
using Zick.GameScheduler.Backend.Data.Models;

namespace Zick.GameScheduler.Backend.API.Hubs;

public class RegistrationHub(IDbContextFactory<ApplicationContext<RacingUserIdentity>> ctx) : Hub
{
    async Task SendRegistrationUpdate(string userId, Guid sessionId)
    {
        var context = await ctx.CreateDbContextAsync();
        
        
        var lobbyCount = context.Sessions
            .Include(x => x.Registrations)
            .First(x => x.Id == sessionId)
            .Registrations.Count;
        
        await Clients.Group(sessionId.ToString()).SendAsync("GetRegistrationCount", userId, lobbyCount);
    }
}