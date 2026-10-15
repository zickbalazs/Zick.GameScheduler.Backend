using Microsoft.Extensions.Logging;
using Zick.GameScheduler.Backend.Data;
using Zick.GameScheduler.Backend.Data.Dashboard.Forms;
using Zick.GameScheduler.Backend.Data.DTOs;
using Zick.GameScheduler.Backend.Data.Models;

namespace Zick.GameScheduler.Backend.Scheduler.Services;

public class DbSessionService(ApplicationContext<RacingUserIdentity> ctx, 
    ISessionSchedulerService schedulerService,
    ILogger<DbSessionService> logger) : ISessionService
{
    public Task<IList<SessionDTO>> GetCustomSessions()
    {
        throw new NotImplementedException();
    }

    public Task<IList<SessionDTO>> GetSessionByLeague(Guid leagueId)
    {
        throw new NotImplementedException();
    }

    public Task<SessionDTO> GetSession(Guid id)
    {
        throw new NotImplementedException();
    }

    public async Task<Guid> AddSession(AddSessionForm form)
    {
        try
        {
            var entry = ctx.Sessions.Add(new()
            {
                Start = form.Start,
                League = ctx.Leagues.First(x=>x.Id == form.LeagueId),
                Track = ctx.Tracks.FirstOrDefault(x=>x.Id == form.TrackId) ?? ctx.Tracks.First(),
                Status = SessionStatus.Waiting
            });
            await ctx.SaveChangesAsync();            
            logger.LogInformation("added new session with id: {id}", entry.Entity.Id);
            return entry.Entity.Id;
        }
        catch (Exception e)
        {
            logger.LogError("session addition failed with: {message}", e.Message);
            return Guid.Empty;
        }
    }

    public Task<(bool Success, string Reason)> UpdateSession(EditSessionForm form)
    {
        throw new NotImplementedException();
    }

    public Task<(bool Success, string Reason)> DeleteSession(Guid id)
    {
        throw new NotImplementedException();
    }

    public Task<bool> CancelSession(Guid id)
    {
        throw new NotImplementedException();
    }

    public async Task<bool> StartSession(Guid id)
    {
        try
        {
            logger.LogInformation("Creating a job for session: {id}", id);
            await schedulerService.CreateSessionFor(id);
            return true;
        }
        catch (Exception e)
        {
            logger.LogError("session start for {sessionId} failed, reason: {message}", id, e.Message);
            return false;
        }
    }
}