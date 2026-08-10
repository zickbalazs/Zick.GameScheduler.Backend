using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace Zick.GameScheduler.Backend.Data.Models;

public class League<TUserIdentity> where TUserIdentity : IdentityUser
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; set; }
    
    [Required]
    public required string Name { get; set; }
    
    public string? Description { get; set; }

    [Required]
    public required SessionDetails SessionSettings { get; set; }
    
    // RELATIONS
    public virtual IList<Session<TUserIdentity>> Sessions { get; } = [];
    
    [Required]
    public virtual Track CurrentTrack { get; set; }
    
    public virtual IList<VehicleClass> AllowedClasses { get; } = [];
}