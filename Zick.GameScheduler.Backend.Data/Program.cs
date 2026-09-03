using System.Runtime.InteropServices;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Zick.GameScheduler.Backend.Data.Models;

namespace Zick.GameScheduler.Backend.Data;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = new HostApplicationBuilder(args);
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows) && builder.Environment.IsDevelopment())
        {
            builder.Services.AddDbContext<ApplicationContext<RacingUserIdentity>>(opt =>
            {
                opt.UseSqlite($"Data Source={Path.Join(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "com.zick.gs", "race.db")}");
            });
        }
        else
        {
            builder.Services.AddDbContext<ApplicationContext<RacingUserIdentity>>(opt =>
            {
                opt.UseNpgsql(Environment.GetEnvironmentVariable("RACE_DB"));
            });    
        }

        var app = builder.Build();
        
        app.Run();
    }
}