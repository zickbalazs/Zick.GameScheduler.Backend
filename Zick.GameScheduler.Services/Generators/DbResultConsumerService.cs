using Microsoft.Extensions.Logging;
using Zick.GameScheduler.Backend.Data;
using Zick.GameScheduler.Backend.Data.Models;
using Zick.GameScheduler.Services.Models;
using Zick.GameScheduler.Services.Utils;

namespace Zick.GameScheduler.Services.Generators;

public class DbResultConsumerService(ApplicationContext<RacingUserIdentity> ctx,
    ILogger<DbResultConsumerService> logger) : IResultConsumerService
{
    public async Task ConsumeAndUploadResult(Guid sessionId, string jsonContent)
    {
        var deserializedContent = JsonConverter.ReadJsonAndSerialize(jsonContent);

        if (deserializedContent is null)
        {
            logger.LogError("results for session {sessionId} could not be consumed with json file content: {jsonContent}",
                sessionId, jsonContent);
            throw new FileLoadException();
        }

        var newEntry = ctx.Results.Add(new()
        {
            Session = ctx.Sessions.First(x=>x.Id == sessionId),
            SessionName = deserializedContent.Type
        });
        
        
        var dbResultEntries = deserializedContent.Results
            .Select(x=>new MappingStruct()
            {
                JsonResult = x,
                DbResult = x
            })
            .ToList();
        
        dbResultEntries.ForEach(x =>
        {
            x.DbResult.User = ctx.Users.First(user => user.SteamId == x.JsonResult.DriverGuid);
            x.DbResult.SessionResult = newEntry.Entity;
        });

        var finalEntries = dbResultEntries
            .Select(x => x.DbResult)
            .OrderBy(y=>y.TotalTime)
            .ToList();

        for (int i = 0; i < finalEntries.Count; i++)
        {
            finalEntries[i].Position = i + 1;
        }

        ctx.ResultDetails.AddRange(finalEntries);

        await ctx.SaveChangesAsync();

    }
}

struct MappingStruct
{
    public ResultItem JsonResult { get; set; }
    public SessionResultDetails<RacingUserIdentity> DbResult { get; set; }
}