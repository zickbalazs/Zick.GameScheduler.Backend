using System.Text.Json.Serialization;

namespace Zick.GameScheduler.Services.Models;

public class AssettoRaceResult
{
    
    public IEnumerable<object> Cars { get; set; }
    public IEnumerable<object> Events { get; set; }
    public IEnumerable<object> Laps { get; set; }
    [JsonPropertyName("Result")]
    public IEnumerable<ResultItem> Results { get; set; }
    public string Type { get; set; }
}