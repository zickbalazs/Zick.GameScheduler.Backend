namespace Zick.GameScheduler.Backend.Data.Dashboard.Forms;

public class AddSessionForm
{
     public Guid LeagueId { get; set; }
     public Guid? TrackId { get; set; }
     public DateTime Start { get; set; }
}