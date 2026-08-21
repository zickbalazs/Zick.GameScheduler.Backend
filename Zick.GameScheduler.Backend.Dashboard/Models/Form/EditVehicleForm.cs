namespace Zick.GameScheduler.Backend.Dashboard.Models.Form;

public class EditVehicleForm
{
    public Guid Id { get; set; }
    public string Model { get; set; }
    public string? Manufacturer { get; set; }
    public string Folder { get; set; }
}