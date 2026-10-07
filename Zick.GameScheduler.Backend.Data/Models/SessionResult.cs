using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace Zick.GameScheduler.Backend.Data.Models;

public class SessionResult<TUser> where TUser : IdentityUser
{
    [Key]
    public Guid Id { get; set; }

    public string SessionName { get; set; }
    public virtual LeagueSession<TUser> Session { get; set; }
    
    public virtual IList<SessionResultDetails<TUser>> Details { get; } = [];
}