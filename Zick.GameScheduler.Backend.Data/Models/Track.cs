using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Zick.GameScheduler.Backend.Data.Models;

public class Track
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; set; }
    
    [Required]
    public string GameId { get; set; }

    [Required]
    public string Country { get; set; }
    
}