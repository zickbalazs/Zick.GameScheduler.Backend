using Microsoft.AspNetCore.Identity;

namespace Zick.GameScheduler.Backend.Data.Models;

public class RacingUserIdentity : IdentityUser
{
    public string? SteamId { get; set; }
    
    // Relations
    public IList<LeagueSession<RacingUserIdentity>> Sessions { get; } = [];
}