using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Zick.GameScheduler.Backend.Data.Models;

public class VehicleClass
{
    [Key]
    [Required]
    public required string ClassIdentifier { get; set; }

    public string Color { get; set; } = "#fff";
    
    public string? ClassName { get; set; }

    // RELATIONS
    public virtual IList<Vehicle> Vehicles { get; } = [];
}