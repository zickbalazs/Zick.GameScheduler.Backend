using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;
using Zick.GameScheduler.Backend.Data.DTOs;

namespace Zick.GameScheduler.Backend.Data.Models;

public class Car<TUserIdentity> : AssettoContent where TUserIdentity : IdentityUser
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; set; }
    public string? Manufacturer { get; set; }
    public required string Model { get; set; }
    
    // Relations
    public virtual IList<RacingClass<TUserIdentity>> Classes { get; } = [];

    public static implicit operator CarDTO(Car<TUserIdentity> car) => new()
    {
        Id = car.Id,
        Manufacturer = car.Manufacturer,
        Model = car.Model
    };
}