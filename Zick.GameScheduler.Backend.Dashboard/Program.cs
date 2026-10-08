using System.Runtime.InteropServices;
using Docker.DotNet;
using Microsoft.EntityFrameworkCore;
using MudBlazor.Services;
using OpenTelemetry.Exporter;
using OpenTelemetry.Logs;
using Quartz;
using Quartz.AspNetCore;
using Zick.GameScheduler.Backend.Dashboard.Common;
using Zick.GameScheduler.Backend.Dashboard.Components;
using Zick.GameScheduler.Backend.Dashboard.Services;
using Zick.GameScheduler.Backend.Data;
using Zick.GameScheduler.Backend.Data.Models;
using Zick.GameScheduler.Backend.EntryListGenerator;
using Zick.GameScheduler.Backend.ResultConsumer;
using Zick.GameScheduler.Backend.Scheduler.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddOpenTelemetry();
builder.Logging.AddOpenTelemetry(opt =>
{
    opt.AddOtlpExporter(x =>
    {
        x.Protocol = OtlpExportProtocol.Grpc;
        x.Endpoint = new Uri("http://localhost:4317");
    });
    opt.IncludeFormattedMessage = true;
    opt.IncludeScopes = true;
});
builder.Services.AddDbContext<ApplicationContext<RacingUserIdentity>>(opt =>
{
    if (!RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
        opt.UseSqlite($"Data Source={Path.Join(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "com.zick.gs", "race.db")}");

    else
        opt.UseNpgsql(Environment.GetEnvironmentVariable("RACE_DB"));
});

// CONTAINER SERVICE
builder.Services.AddScoped<IDockerClient>(_ =>
    new DockerClientConfiguration(new Uri(Environment.GetEnvironmentVariable("DOCKER_URL")!)).CreateClient());
builder.Services.AddScoped<IPortClaimService, DbPortClaimService>();
builder.Services.AddScoped<IContainerService, DockerEngineContainerService>();
builder.Services.AddScoped<IResultConsumerService, DbResultConsumerService>();

// LOCAL SERVICES
builder.Services.AddScoped<IVehicleService, DbVehicleService>();
builder.Services.AddScoped<ITrackService, DbTrackService>();
builder.Services.AddScoped<IRacingClassService, DbRacingClassService>();
builder.Services.AddScoped<ILeagueService, DbLeagueService>();
builder.Services.AddScoped<ISessionStatusUpdateService, DbSessionStatusUpdateService>();

// SCHEDULER SERVICES
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
    opt.WaitForJobsToComplete = false;
});
builder.Services.AddScoped<ISessionSchedulerService, QuartzSchedulerService>();
builder.Services.AddScoped<ISessionService, DbSessionService>();
builder.Services.AddScoped<IEntryGenerator<RacingUserIdentity>, DbEntryGenerator>();

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();
builder.Services.AddMudServices();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    var ctx = app.Services.CreateScope().ServiceProvider.GetService<ApplicationContext<RacingUserIdentity>>()!;

    var customLeague = ctx.Leagues.FirstOrDefault(x => x.Id == Constants.CustomLeagueId);
    var customClass = ctx.Classes.FirstOrDefault(x => x.Abbreviation == Constants.CustomRacingClass);
    var firstTrack = ctx.Tracks.FirstOrDefault();

    if (firstTrack is null)
    {
        var trackEntry = ctx.Tracks.Add(new()
        {
            CountryCode = "BEL",
            Name = "Spa",
            FolderName = "/content/tracks/ks_spa"
        });
        ctx.SaveChanges();
        firstTrack = trackEntry.Entity;
        
    }
    
    if (customClass is null)
    {
        var classDbEntry = ctx.Classes.Add(new()
        {
            Abbreviation = Constants.CustomRacingClass,
            Name = "Custom Races Class"
        });
        ctx.SaveChanges();
        customClass = classDbEntry.Entity;
    }
    
    if (customLeague is null)
    {
        var leagueDbEntry = ctx.Leagues.Add(new()
        {
            Id = Constants.CustomLeagueId,
            Name = "Custom Racing League",
            Start = DateTime.Now,
            End = DateTime.MaxValue,
            CurrentTrack = firstTrack,
            Interval = TimeSpan.FromDays(365),
            Practice = new()
            {
                DurationInMinutes  = 20,
                SessionName = "Practice"
            },
            Qualify = new()
            {
                DurationInMinutes = 12,
                SessionName = "Qualifying"
            },
            Race = new()
            {
                SessionName = "Main Event",
                DurationInMinutes = 30
            }
        });
        leagueDbEntry.Entity.Classes.Add(customClass);
        ctx.SaveChanges();
    }

    if (builder.Configuration.GetSection("Docker") is null)
    {
        throw new ArgumentNullException();
    }
    
}
// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
app.UseHttpsRedirection();

app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();