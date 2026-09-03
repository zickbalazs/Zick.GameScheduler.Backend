namespace Zick.GameScheduler.Backend.Dashboard.Models.Form;

public class EditTrackForm
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string? CountryCode { get; set; }
    public string Folder { get; set; }
}