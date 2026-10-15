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
using Zick.GameScheduler.Backend.Data;
using Zick.GameScheduler.Backend.Data.Models;
using Zick.GameScheduler.Services.Generators;
using Zick.GameScheduler.Backend.Scheduler.Services;
using Zick.GameScheduler.Services.Data;
using DbSessionService = Zick.GameScheduler.Services.Data.DbSessionService;
using ISessionService = Zick.GameScheduler.Services.Data.ISessionService;

var builder = WebApplication.CreateBuilder(args);

// TODO LIST:
// - Logging (page data)
// - switch out DbContext to DbContextFactory for thread safety

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

builder.Services.AddDbContextFactory<ApplicationContext<RacingUserIdentity>>(opt =>
{
    if (!RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
        opt.UseSqlite($"Data Source={Path.Join(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "com.zick.gs", "race.db")}");

    else
        opt.UseNpgsql(Environment.GetEnvironmentVariable("RACE_DB"));
});


// LOCAL SERVICES
builder.Services.AddScoped<IVehicleService, DbVehicleService>();
builder.Services.AddScoped<ITrackService, DbTrackService>();
builder.Services.AddScoped<IRacingClassService, DbRacingClassService>();
builder.Services.AddScoped<ILeagueService, DbLeagueService>();


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