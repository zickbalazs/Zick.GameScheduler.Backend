namespace Zick.GameScheduler.Backend.Data.Dashboard.Forms;

public class EditTrackForm
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string? CountryCode { get; set; }
    public string Folder { get; set; }
}