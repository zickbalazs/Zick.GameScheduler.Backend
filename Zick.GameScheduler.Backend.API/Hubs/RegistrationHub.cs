using System.Text.RegularExpressions;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Zick.GameScheduler.Backend.Data;
using Zick.GameScheduler.Backend.Data.Models;

namespace Zick.GameScheduler.Backend.API.Hubs;

public class RegistrationHub(ApplicationContext<RacingUserIdentity> ctx) : Hub
{
    async Task SendSessionLobbySize(Guid sessionId)
    {
        var lobbyCount = ctx.Registrations
            .Include(x => x.Session)
            .Where(x => x.Session.Id == sessionId);
        
        await Clients.Group(sessionId.ToString()).SendAsync("LobbySizeChange", sessionId.ToString(), lobbyCount);
    }

    async Task JoinLobby(Guid sessionId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, sessionId.ToString());
    }
}