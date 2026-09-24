namespace Zick.GameScheduler.Backend.Dashboard.Models.Form;

public class AddSessionForm
{
     public Guid LeagueId { get; set; }
     public Guid? TrackId { get; set; }
     public DateTime Start { get; set; }
}