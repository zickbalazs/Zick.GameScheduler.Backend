using System.ComponentModel.DataAnnotations;

namespace Zick.GameScheduler.Backend.API.Models;

public class SessionRegisterBody
{
    [Required]
    public Guid CarId { get; set; }
}