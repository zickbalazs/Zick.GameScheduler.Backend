using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Zick.GameScheduler.Backend.Data.Models;

public class Vehicle
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; set; }
    
    [Required]
    public string Manufacturer { get; set; }
    
    [Required]
    public string ModelName { get; set; }
    
    [Required]
    public string GameId { get; set; }

    // RELATIONS
    public virtual IList<VehicleClass> Classes { get; } = [];

}