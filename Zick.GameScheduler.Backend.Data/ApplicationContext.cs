using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Zick.GameScheduler.Backend.Data.Models;

namespace Zick.GameScheduler.Backend.Data;

public class ApplicationContext<TUserIdentity>(DbContextOptions options) 
    : IdentityDbContext<TUserIdentity>(options)
    where TUserIdentity : IdentityUser
{
    public DbSet<Track> Tracks { get; set; }
    public DbSet<Vehicle> Vehicles { get; set; }
    public DbSet<VehicleClass> Classes { get; set; }
    public DbSet<League<TUserIdentity>> Leagues { get; set; }
    public DbSet<Session<TUserIdentity>> Sessions { get; set; }
}
