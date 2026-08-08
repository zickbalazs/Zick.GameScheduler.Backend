using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace Zick.GameScheduler.Backend.Data.Models;

public class Session<TUserIdentity> where TUserIdentity : IdentityUser
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; set; }

    public DateTime Start { get; set; }
    
    public SessionState State { get; set; } = SessionState.Waiting;
    
    // Relations
    [Required]
    public virtual League<TUserIdentity> League { get; set; }

    public virtual IList<TUserIdentity> Registrations { get; } = [];
}

public enum SessionState
{
    Waiting,
    Initializing,
    Underway,
    Cancelled,
    Finished
}