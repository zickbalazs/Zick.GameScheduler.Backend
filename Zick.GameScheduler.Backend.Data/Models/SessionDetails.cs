using System.ComponentModel.DataAnnotations.Schema;

namespace Zick.GameScheduler.Backend.Data.Models;

[ComplexType]
public class SessionDetails
{
    public int? Laps { get; set; }
    public TimeSpan? RaceTime { get; set; }
    public int LobbySize { get; set; }
}