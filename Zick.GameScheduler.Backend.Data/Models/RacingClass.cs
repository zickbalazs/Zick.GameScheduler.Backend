using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;
using Zick.GameScheduler.Backend.Data.DTOs;

namespace Zick.GameScheduler.Backend.Data.Models;

public class RacingClass<TUserIdentity> where TUserIdentity : IdentityUser
{
    [Key]
    [Required]
    public required string Abbreviation { get; set; }
    public string? Color { get; set; }
    public string? Name { get; set; }
    
    
    // Relations
    public virtual IList<Car<TUserIdentity>> Cars { get; } = [];
    public virtual IList<League<TUserIdentity>> Leagues { get; } = [];

    public static implicit operator RacingClassDTO(RacingClass<TUserIdentity> rClass) => new()
    {
        Abbreviation = rClass.Abbreviation,
        Name = rClass.Name,
        Color = rClass.Name,
        Cars = [ ..rClass.Cars ]
    };
}