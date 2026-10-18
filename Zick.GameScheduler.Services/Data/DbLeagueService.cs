using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Zick.GameScheduler.Backend.Data;
using Zick.GameScheduler.Backend.Data.Dashboard.Forms;
using Zick.GameScheduler.Backend.Data.DTOs;
using Zick.GameScheduler.Backend.Data.Models;

namespace Zick.GameScheduler.Services.Data;

public class DbLeagueService(IDbContextFactory<ApplicationContext<RacingUserIdentity>> contextFactory, 
    ILogger<DbLeagueService> logger) : ILeagueService
{
    public async Task AddLeague(AddLeagueForm form, bool fillSessions)
    {
        if (form.CurrentTrack == null)
            throw new ArgumentNullException();

        if (form.FromToRange.Start == null || form.FromToRange.End == null)
            throw new ArgumentNullException();
        
        var dbEntry = new League<RacingUserIdentity>
        {
            Name = form.Name,
            Description = form.Description,
            Interval = form.Interval,
            Start = (DateTime)form.FromToRange.Start,
            End = (DateTime)form.FromToRange.End,
            Practice = form.PracticeData,
            Qualify = form.QualifyData,
            Race = form.Race,
        };

        
        
        await using (var ctx = await contextFactory.CreateDbContextAsync())
        {
            dbEntry.CurrentTrack = ctx.Tracks.First(x => x.Id == form.CurrentTrack.Id);
            foreach (var cls in form.RacingClasses)
            {
                var classEntry = ctx.Classes.First(x => x.Abbreviation == cls.Abbreviation);
                dbEntry.Classes.Add(classEntry);
            }

            ctx.Leagues.Add(dbEntry);
            await ctx.SaveChangesAsync();
        }
    }

    public async Task DeleteLeague(Guid id)
    {
        await using (var ctx = await contextFactory.CreateDbContextAsync())
        {
            var league = ctx.Leagues
                .Include(x=>x.Sessions)
                .First(x => x.Id == id);
            ctx.Leagues.Remove(league);
            await ctx.SaveChangesAsync();
            logger.LogInformation("deleted league with {sessionCount} sessions", league.Sessions.Count);
        }
    }

    public Task ModifyLeague(EditLeagueForm form)
    {
        throw new NotImplementedException();
    }

    public async Task<IList<LeagueDTO>> GetLeagues()
    {
        await using (var ctx = await contextFactory.CreateDbContextAsync())
        {
            return
            [
                .. ctx.Leagues
                    .Include(x => x.Sessions)
                    .ThenInclude(x => x.Registrations)
                    .Include(x => x.CurrentTrack)
                    .Include(x => x.Classes)
            ];

        }
    }

    public async Task<LeagueDTO> GetLeagueById(Guid id)
    {
        await using (var ctx = await contextFactory.CreateDbContextAsync())
        {
            return await ctx.Leagues
                .Include(x => x.Sessions)
                .ThenInclude(x => x.Registrations)
                .Include(x => x.CurrentTrack)
                .Include(x => x.Classes)
                .FirstAsync(x => x.Id == id);
        }
    }

    public async Task ChangeActiveTrack(Guid id, TrackDTO track)
    {
        await using (var ctx = await contextFactory.CreateDbContextAsync())
        {
            var dbEntry = await ctx.Leagues
                .Include(x=>x.CurrentTrack)
                .FirstAsync(x => x.Id == id);

            dbEntry.CurrentTrack = await ctx.Tracks.FirstAsync(x => x.Id == track.Id);

            await ctx.SaveChangesAsync();
        }
    }

    public Task RemoveSessions(Guid id)
    {
        throw new NotImplementedException();
    }

    public async Task AddSessions(Guid id, bool destructive)
    {
        if (destructive)
        {
            await using (var ctx = await contextFactory.CreateDbContextAsync())
            {
                var sessionsToRemove = ctx.Sessions
                    .Include(x => x.League)
                    .Where(x => x.League.Id == id && x.Status == SessionStatus.Waiting);
                
                ctx.Sessions.RemoveRange(sessionsToRemove);
                var results = await ctx.SaveChangesAsync();
                logger.LogInformation("deleted {sessionCount} sessions that were waiting", results);
            }
        }
        await using (var ctx = await contextFactory.CreateDbContextAsync())
        {
            var league = ctx.Leagues.First(x => x.Id == id);
            var times = ctx.Sessions
                .Include(m => m.League)
                .Where(x => x.League.Id == id);

            DateTime time = times.Any() ? times.Max(x => x.Start) : league.Start; 

            time += league.Interval;
            
            DateTime end = league.End;


            while (time < end)
            {
                ctx.Sessions.Add(new()
                {
                    Start = time,
                    League = league,
                    Track = null,
                    PortClaim = null
                });
                time += league.Interval;
            }

            var results = await ctx.SaveChangesAsync();
            logger.LogInformation("added {sessionCount} sessions for league", results);
        }
    }
}