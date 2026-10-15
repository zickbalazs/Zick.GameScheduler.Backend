using MudBlazor;
using Zick.GameScheduler.Backend.Data.DTOs;
using Zick.GameScheduler.Backend.Data.Models;

namespace Zick.GameScheduler.Backend.Data.Dashboard.Forms;

public class EditLeagueForm
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string? Description { get; set; }
    public IEnumerable<RacingClassDTO> RacingClasses { get; set; } = [];
    public TrackDTO? ActiveTrack { get; set; }
    public SessionData? Practice { get; set; }
    public SessionData? Qualify { get; set; }
    public SessionData? Race { get; set; }
    public DateRange StartEndRange { get; set; } = new();
    public TimeSpan Interval { get; set; }
}