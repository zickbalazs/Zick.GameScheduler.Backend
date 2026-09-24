using System.Runtime.InteropServices;
using Docker.DotNet;
using Microsoft.EntityFrameworkCore;
using MudBlazor.Services;
using Quartz;
using Quartz.AspNetCore;
using Zick.GameScheduler.Backend.Dashboard.Components;
using Zick.GameScheduler.Backend.Dashboard.Services;
using Zick.GameScheduler.Backend.Data;
using Zick.GameScheduler.Backend.Data.Models;
using Zick.GameScheduler.Backend.Scheduler.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddOpenTelemetry();
builder.Logging.AddOpenTelemetry(opt =>
{
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
// LOCAL SERVICES
builder.Services.AddScoped<IVehicleService, DbVehicleService>();
builder.Services.AddScoped<ITrackService, DbTrackService>();
builder.Services.AddScoped<IRacingClassService, DbRacingClassService>();
builder.Services.AddScoped<ILeagueService, DbLeagueService>();

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

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();
builder.Services.AddMudServices();

var app = builder.Build();

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