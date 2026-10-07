using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace Zick.GameScheduler.Backend.Data.Models;

public class SessionResultDetails<TUser> where TUser : IdentityUser
{
    [Key]
    public Guid Id { get; set; }
    
    public int Position { get; set; }
    
    public bool IsDisqualified { get; set; }
    public bool IsPenalized { get; set; }
    
    public TimeSpan PenaltyTime { get; set; }
    public TimeSpan BestLapTime { get; set; }
    public TimeSpan TotalTime { get; set; }
    
    public string CarName { get; set; }
    
    public virtual TUser User { get; set; }
    public virtual SessionResult<TUser> SessionResult { get; set; }
}