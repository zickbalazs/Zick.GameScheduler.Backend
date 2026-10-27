using System.Runtime.InteropServices;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Zick.GameScheduler.Backend.Data.Models;

namespace Zick.GameScheduler.Backend.Data;

public class Program
{
    
    public static void Main(string[] args)
    {
        var builder = new HostApplicationBuilder(args);
        builder.Services.AddDbContext<ApplicationContext<RacingUserIdentity>>(opt =>
        {
            opt.UseNpgsql(builder.Configuration.GetConnectionString("GameDb"));
        });    
        var app = builder.Build();
        
        app.Run();
    }
}