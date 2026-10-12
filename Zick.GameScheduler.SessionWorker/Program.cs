using System.Runtime.InteropServices;
using Microsoft.EntityFrameworkCore;
using Quartz;
using Quartz.AspNetCore;
using Zick.GameScheduler.Backend.Data;
using Zick.GameScheduler.Backend.Data.Models;
using Zick.GameScheduler.SessionWorker;

var builder = Host.CreateApplicationBuilder(args);

if (builder.Configuration.GetValue<int?>("SessionProcessor:BatchLimit") == null)
    throw new ArgumentNullException("SessionProcessor:BatchLimit", "Batch limit must be set in application settings");

builder.Services.AddDbContextFactory<ApplicationContext<RacingUserIdentity>>(opt =>
{
    if (!RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
        opt.UseSqlite($"Data Source={Path.Join(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "com.zick.gs", "race.db")}");

    else
        opt.UseNpgsql(Environment.GetEnvironmentVariable("RACE_DB"));
});

builder.Services.AddLogging(opt =>
{
    opt.AddConsole();
});

builder.Services.AddHostedService<SessionProcessorWorker>();
builder.Services.AddQuartz(opt =>
{
    opt.UsePersistentStore(o =>
    {
        o.UseSystemTextJsonSerializer();
        o.UseProperties = true;
        
        if (!RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            o.UseSQLite($"Data Source={Path.Join(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "com.zick.gs", "sched.db")}");
        else
            o.UsePostgres(Environment.GetEnvironmentVariable("RACE_SCHED_DB") ?? 
                          throw new KeyNotFoundException("Connection String not found!"));
    });
});
builder.Services.AddQuartzServer(opt =>
{
    opt.AwaitApplicationStarted = true;
});

var host = builder.Build();

host.Run();