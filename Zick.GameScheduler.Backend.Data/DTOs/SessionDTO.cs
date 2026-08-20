namespace Zick.GameScheduler.Backend.Data.DTOs;

public class SessionDTO 
{
    public Guid Id { get; set; }
    public DateTime Start { get; set; }
    public IList<string> RegistrationIds { get; set; } = [];
    public Guid LeagueId { get; set; }
    public Guid? TrackId { get; set; }
}