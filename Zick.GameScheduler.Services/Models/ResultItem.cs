using Zick.GameScheduler.Backend.Data.Models;

namespace Zick.GameScheduler.Services.Models;


public class ResultItem
{
    public int BallastKG { get; set; }
    public int BestLap { get; set; }
    public int CarId { get; set; }
    public string CarModel { get; set; }
    public string DriverGuid { get; set; }
    public string DriverName { get; set; }
    public int? Restrictor { get; set; }
    public int TotalTime { get; set; }
    public bool HasPenalty { get; set; }
    public int PenaltyTime { get; set; }
    public int LapPenalty { get; set; }
    public bool Disqualified { get; set; }

    public static implicit operator SessionResultDetails<RacingUserIdentity>(ResultItem item)
    {
        return new()
        {
            Position = 0,
            IsDisqualified = item.Disqualified,
            IsPenalized = item.HasPenalty,
            PenaltyTime = TimeSpan.FromMilliseconds(item.PenaltyTime),
            BestLapTime = TimeSpan.FromMilliseconds(item.BestLap),
            TotalTime = TimeSpan.FromMilliseconds(item.TotalTime),
            CarName = item.CarModel,
            User = default,
            SessionResult = null
        };
    }
}

