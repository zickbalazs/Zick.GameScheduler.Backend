using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.CompilerServices;
using Microsoft.AspNetCore.Identity;
using Zick.GameScheduler.Backend.Data.DTOs;

namespace Zick.GameScheduler.Backend.Data.Models;

public class Track<TUserIdentity> : AssettoContent where TUserIdentity : IdentityUser
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; set; }
    
    public required string Name { get; set; }
    public string? CountryCode { get; set; }

    // Relations
    public IList<League<TUserIdentity>> CurrentEvents { get; } = [];
    public IList<LeagueSession<TUserIdentity>> Sessions { get; } = [];

    public static implicit operator TrackDTO(Track<TUserIdentity> track) => new()
    {
        Id = track.Id,
        Name = track.Name,
        CountryCode = track.CountryCode,
        Folder = track.FolderName
    };
}