using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;
using Zick.GameScheduler.Backend.Data.DTOs;

namespace Zick.GameScheduler.Backend.Data.Models;

public class League<TUserIdentity> where TUserIdentity : IdentityUser
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; set; }
    
    public required string Name { get; set; }
    public string? Description { get; set; }

    public TimeSpan Interval { get; set; } = TimeSpan.FromMinutes(60);
    public DateTime Start { get; set; } = DateTime.Now;
    public DateTime End { get; set; } = DateTime.Now + TimeSpan.FromDays(10);
    
    public SessionData? Practice { get; set; }
    public SessionData? Qualify { get; set; }
    public SessionData? Race { get; set; }
    
    
    // Relations
    public virtual Track<TUserIdentity> CurrentTrack { get; set; }
    public virtual IList<RacingClass<TUserIdentity>> Classes { get; } = [];

    public static implicit operator LeagueDTO(League<TUserIdentity> league) => new()
    {
        Id = league.Id,
        Name = league.Name,
        Description = league.Description,
        Interval = league.Interval,
        Start = league.Start,
        End = league.End,
        Practice = league.Practice,
        Qualify = league.Qualify,
        Race = league.Race,
        CurrentTrackId = league.CurrentTrack.Id,
        CurrentClassIds = [ ..league.Classes.Select(x=>x.Abbreviation) ]
    };


}