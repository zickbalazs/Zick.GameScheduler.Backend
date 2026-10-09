using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Zick.GameScheduler.Backend.Data;
using Zick.GameScheduler.Backend.Data.Models;

namespace Zick.GameScheduler.Services.Generators;

public class DbEntryGenerator(ILogger<DbEntryGenerator> logger,
    IConfiguration config,
    ApplicationContext<RacingUserIdentity> ctx) : IEntryGenerator<RacingUserIdentity>
{
    
    public async Task<string> GenerateEntryListForSessions(Guid sessionId)
    {
        IList<string> entries = [];
        
        var session = ctx.Sessions
            .Include(x=>x.Registrations)
            .ThenInclude(x=>x.Car)
            .Include(x=>x.Registrations)
            .ThenInclude(x=>x.User)
            .First(x => x.Id == sessionId);

        for (int i = 0; i < session.Registrations.Count; i++)
        {
            var registration = session.Registrations[i];
            entries.Add(registration.CreateEntryListEntry(i, registration.User.SteamId!));
        }
        logger.LogInformation("entry generated, {entryList}", string.Join("\n\n", entries));
        return string.Join("\n\n", entries);
    }

    public Task<string> CreateEntryListForRestart(Guid sessionId)
    {
        // TODO
        throw new NotImplementedException();
    }
    
    
    
}