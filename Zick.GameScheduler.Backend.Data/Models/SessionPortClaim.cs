using System.ComponentModel.DataAnnotations;

namespace Zick.GameScheduler.Backend.Data.Models;

public class SessionPortClaim
{
    [Key]
    public Guid Id { get; set; }
    public string ClaimedPort { get; set; }
    public string ClaimedHttpPort { get; set; }
    public bool IsInUse { get; set; }
}