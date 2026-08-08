using Microsoft.AspNetCore.Identity;

namespace Zick.GameScheduler.Backend.Data.Models;

public class RacingUser : IdentityUser
{
    public string? FirstName { get; set; }
    public string? LastName { get; set; }

    public virtual IList<Session<RacingUser>> Sessions { get; } = [];
}