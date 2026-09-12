using MudBlazor;
using Zick.GameScheduler.Backend.Data.DTOs;
using Zick.GameScheduler.Backend.Data.Models;

namespace Zick.GameScheduler.Backend.Dashboard.Models.Form;

public class AddLeagueForm
{
    public string Name { get; set; }
    
    public string? Description { get; set; }
    
    public SessionData? PracticeData { get; set; }
    
    public SessionData? QualifyData { get; set; }

    public SessionData? Race { get; set; }
    
    public TimeSpan Interval { get; set; }

    public DateRange Duration { get; set; } = new();
    
    public TrackDTO? CurrentTrack { get; set; }

    public IList<RacingClassDTO> RacingClasses { get; set; } = [];
}