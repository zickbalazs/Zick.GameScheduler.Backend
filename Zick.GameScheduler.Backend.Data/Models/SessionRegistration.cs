using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace Zick.GameScheduler.Backend.Data.Models;

public class SessionRegistration<TUser> where TUser : IdentityUser
{
    [Key]
    public Guid Id { get; set; }
    
    public virtual TUser User { get; set; }
    public virtual LeagueSession<TUser> Session { get; set; }
    public virtual Car<TUser> Car { get; set; }

    public string CreateEntryListEntry(int index, string steamId)
    {
        return $"[CAR_{index}]\n" +
               $"MODEL={Car.FolderName}\n" +
               $"DRIVERNAME=\n" +
               $"TEAM=\n" +
               $"GUID={steamId}\n" +
               $"SPECTATOR_MODE=0\n" +
               $"BALLAST=0";
    }
    
}