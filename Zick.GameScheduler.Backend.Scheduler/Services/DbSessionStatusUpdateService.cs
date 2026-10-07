using Microsoft.Extensions.Logging;
using Zick.GameScheduler.Backend.Data;
using Zick.GameScheduler.Backend.Data.Models;

namespace Zick.GameScheduler.Backend.Scheduler.Services;

public class DbSessionStatusUpdateService(ApplicationContext<RacingUserIdentity> ctx,
    ILogger<DbSessionStatusUpdateService> logger) : ISessionStatusUpdateService
{
    public async Task UpdateSessionWithStatus(Guid sessionId, SessionStatus toStatus)
    {
        var entry = ctx.Sessions.First(x => x.Id == sessionId);
        logger.LogDebug("Updating session: {sessionId} from {fromStatus} to {toStatus}", sessionId, entry.Status, toStatus);
        entry.Status = toStatus;
        await ctx.SaveChangesAsync();
    }
}