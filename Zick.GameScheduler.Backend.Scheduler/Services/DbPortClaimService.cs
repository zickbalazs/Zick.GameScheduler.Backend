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
        var claimsInUse = ctx.Sessions
            .Include(x => x.PortClaim)
            .Where(x=>x.PortClaim != null && x.PortClaim.IsInUse)
            .Select(x=>x.PortClaim).ToList();

        var allowedPorts =
            Enumerable.Range(config.GetValue<int>("Docker:MinPort"), config.GetValue<int>("Docker:MaxPort"))
                .Except(claimsInUse.Select(x=>int.Parse(x!.ClaimedPort))).ToList();

        var rolledPort = allowedPorts[RandomNumberGenerator.GetInt32(0, allowedPorts.Count)];

        ctx.PortClaims.Add(new()
        {
            IsInUse = true,
            ClaimedPort = $"{rolledPort}"
        });
        await ctx.SaveChangesAsync();
        logger.LogInformation("[session {sessionId}]: rolled port {port} for session", id, rolledPort);
        return $"{rolledPort}";
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