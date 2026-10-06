using System.Collections.Immutable;
using System.Security.Cryptography;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Zick.GameScheduler.Backend.Data;
using Zick.GameScheduler.Backend.Data.Models;

namespace Zick.GameScheduler.Backend.Scheduler.Services;

public class DbPortClaimService(ApplicationContext<RacingUserIdentity> ctx,
    IConfiguration config,
    ILogger<DbPortClaimService> logger) : IPortClaimService
{
    public async Task<string> ClaimPortForSession(Guid id)
    {
        var claimsInUse = ctx.PortClaims.Where(x => x.IsInUse);

        var httpPorts = claimsInUse.Select(x => int.Parse(x.ClaimedHttpPort));
        var serverPorts = claimsInUse.Select(x => int.Parse(x.ClaimedPort));
        
        var allowedPorts =
            Enumerable.Sequence(config.GetValue<int>("Docker:MinPort"), config.GetValue<int>("Docker:MaxPort"), step: 1)
                .Except(httpPorts).Except(serverPorts).ToList();

        var rolledPort = allowedPorts[RandomNumberGenerator.GetInt32(0, allowedPorts.Count)];
        allowedPorts.Remove(rolledPort);
        var rolledHttp = allowedPorts[RandomNumberGenerator.GetInt32(0, allowedPorts.Count)];
        
        
        
        ctx.PortClaims.Add(new()
        {
            IsInUse = true,
            ClaimedPort = $"{rolledPort}",
            ClaimedHttpPort = $"{rolledHttp}"
        });
        
        await ctx.SaveChangesAsync();
        logger.LogInformation("[session {sessionId}]: rolled ports {port} for session", id, rolledPort);
        return $"{rolledPort};{rolledHttp}";
    }

    public async Task EndClaimForSession(Guid id)
    {
        var sessionClaim = ctx.Sessions
            .Include(x => x.PortClaim)
            .First(x => x.Id == id).PortClaim;

        if (sessionClaim is null)
        {
            logger.LogError("[session {sessionId}]: claim for session was not found, check db and logs", id);
            return;
        }
        
        var claimEntry = ctx.PortClaims.First(x => x.Id == sessionClaim.Id);
        claimEntry.IsInUse = false;
        await ctx.SaveChangesAsync();
        logger.LogInformation("[session {sessionId}]: marking session claim port: {port} as not used", id, claimEntry.ClaimedPort);
    }
}