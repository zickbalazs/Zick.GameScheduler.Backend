using System.Text.Json;
using Zick.GameScheduler.Backend.Data.Models;
using Zick.GameScheduler.Services.Models;

namespace Zick.GameScheduler.Services.Utils;

public static class JsonConverter
{
    public static AssettoRaceResult? ReadJsonAndSerialize(string fileContent)
    {
        return JsonSerializer.Deserialize<AssettoRaceResult>(fileContent);
    }


    public static SessionResult<RacingUserIdentity> MapRaceResultToDbResults(AssettoRaceResult gameResult)
    {
        var results = new SessionResult<RacingUserIdentity>();



        return results;
    }
}