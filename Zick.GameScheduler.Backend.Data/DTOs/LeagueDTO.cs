using Zick.GameScheduler.Backend.Data.Models;

namespace Zick.GameScheduler.Backend.Data.DTOs;

public class LeagueDTO
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string? Description { get; set; }
    public TimeSpan Interval { get; set; }
    public DateTime Start { get; set; }
    public DateTime End { get; set; }
    public SessionData? Practice { get; set; }
    public SessionData? Qualify { get; set; }
    public SessionData? Race { get; set; }
    public Guid CurrentTrackId { get; set; }
    public IList<string> CurrentClassIds { get; set; } = [];
}