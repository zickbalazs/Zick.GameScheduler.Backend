using Microsoft.EntityFrameworkCore;
using Zick.GameScheduler.Backend.Data;
using Zick.GameScheduler.Backend.Data.Models;

namespace Zick.GameScheduler.Services.Data;

public class DbSessionService(IDbContextFactory<ApplicationContext<RacingUserIdentity>> ctxFactory) : ISessionService
{
    public async Task DeleteSession(Guid id)
    {
        await using (var ctx = await ctxFactory.CreateDbContextAsync())
        {
            ctx.Sessions.Remove(ctx.Sessions.First(x => x.Id == id));
            await ctx.SaveChangesAsync();
        }
    }

    public async Task CancelSession(Guid id)
    {
        await using (var ctx = await ctxFactory.CreateDbContextAsync())
        {
            var session = ctx.Sessions.First(x => x.Id == id);
            if (SessionStatus.Cancelled == session.Status)
                session.Status = SessionStatus.Waiting;
            else
                session.Status = SessionStatus.Cancelled;
            await ctx.SaveChangesAsync();
        }
    }

    public Task StartSessionNow(Guid id)
    {
        throw new NotImplementedException();
    }
}