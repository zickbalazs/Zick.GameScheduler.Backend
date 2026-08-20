using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;
using Zick.GameScheduler.Backend.Data.DTOs;

namespace Zick.GameScheduler.Backend.Data.Models;

public class LeagueSession<TUserIdentity> where TUserIdentity : IdentityUser
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; set; }

    public SessionStatus Status { get; set; } = SessionStatus.Waiting;    
    public DateTime Start { get; set; }

    // Relations
    public virtual IList<TUserIdentity> Registrations { get; } = [];
    public virtual League<TUserIdentity> League { get; set; }
    public virtual Track<TUserIdentity>? Track { get; set; }

    public static implicit operator SessionDTO(LeagueSession<TUserIdentity> session) => new()
    {
        Id = session.Id,
        Start = session.Start,
        RegistrationIds = [ ..session.Registrations.Select(x=>x.Id) ],
        LeagueId = session.League.Id,
        TrackId = session.Track?.Id
    };
}


public enum SessionStatus
{
    Waiting,
    Starting,
    Underway,
    Finished,
    Cancelled,
    Error
}