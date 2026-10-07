using System.Text.Json;
using Zick.GameScheduler.Backend.Data.Models;
using Zick.GameScheduler.Backend.ResultConsumer.Models;

namespace Zick.GameScheduler.Backend.ResultConsumer.Utils;

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