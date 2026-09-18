using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Zick.GameScheduler.Backend.Data.Models;

namespace Zick.GameScheduler.Backend.Data;

public class ApplicationContext<TUserIdentity>(DbContextOptions options) 
    : IdentityDbContext<TUserIdentity>(options)
    where TUserIdentity : IdentityUser
{
    public DbSet<Car<TUserIdentity>> Cars { get; set; }
    public DbSet<RacingClass<TUserIdentity>> Classes { get; set; }
    public DbSet<Track<TUserIdentity>> Tracks { get; set; }
    public DbSet<League<TUserIdentity>> Leagues { get; set; }
    public DbSet<LeagueSession<TUserIdentity>> Sessions { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.Entity<League<TUserIdentity>>().ComplexProperty(x => x.Practice);
        builder.Entity<League<TUserIdentity>>().ComplexProperty(x => x.Qualify);
        builder.Entity<League<TUserIdentity>>().ComplexProperty(x => x.Race);
    }
}
