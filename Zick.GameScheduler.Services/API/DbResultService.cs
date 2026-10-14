using Microsoft.EntityFrameworkCore;
using Zick.GameScheduler.Backend.Data;
using Zick.GameScheduler.Backend.Data.DTOs;
using Zick.GameScheduler.Backend.Data.Models;

namespace Zick.GameScheduler.Services.ServicesAPI;

public class DbResultService(ApplicationContext<RacingUserIdentity> ctx) : IResultService
{
    public async Task<ResultDTO> GetResultForSession(Guid sessionId)
    {
        var results = await ctx
            .Results
            .Include(x => x.Session)
            .Include(x => x.Details)
            .ThenInclude(x => x.User)
            .Include(x => x.Details)
            .FirstAsync(x => x.Id == sessionId);
        return new();
    }
}