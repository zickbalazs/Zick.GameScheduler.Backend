using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Zick.GameScheduler.Backend.Data;
using Zick.GameScheduler.Backend.Data.Models;

namespace Zick.GameScheduler.Backend.Scheduler.Services;

public class QuartzSchedulerService(ApplicationContext<RacingUserIdentity> ctx, ILogger<QuartzSchedulerService> logger) : ISessionSchedulerService
{
    public async Task CreateSessionFor(Guid sessionId)
    {
        logger.LogTrace("Starting job for session with id: {sessionId}", sessionId);

        try
        {
            var session = await ctx.Sessions
                .FirstOrDefaultAsync(x => x.Id == sessionId);
            
            if (session is null)
                logger.LogError("session is not found in db");
            else
                logger.LogInformation("found session in db");
            
        }
        catch (Exception e)
        {
            logger
                .LogError("job failed with error type of {errorType} and with message: {errorMessage}", 
                    e.GetType().Name, 
                    e.Message);
        }
        
        
    }

    public Task CreateWatcherForSession(Guid sessionId)
    {
        throw new NotImplementedException();
    }
}