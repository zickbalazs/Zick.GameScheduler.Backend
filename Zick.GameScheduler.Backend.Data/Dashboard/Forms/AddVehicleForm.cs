using System.ComponentModel.DataAnnotations;

namespace Zick.GameScheduler.Backend.Data.Dashboard.Forms;

public class AddVehicleForm
{
    [Required]
    public required string Model { get; set; }
    
    public string? Manufacturer { get; set; }
    
    [Required]
    public required string FolderName { get; set; }
}