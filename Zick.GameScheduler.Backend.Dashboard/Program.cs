using System.Runtime.InteropServices;
using Docker.DotNet;
using Microsoft.EntityFrameworkCore;
using MudBlazor.Services;
using OpenTelemetry.Exporter;
using OpenTelemetry.Logs;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
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

string dbString = builder.Configuration.GetConnectionString("GameDatabase")
                  ?? throw new ArgumentException("GameDb connection was not set."),
       logUrl   = builder.Configuration.GetValue<string>("Telemetry:LogEndpoint") 
                  ?? throw new ArgumentException("Telemetry.LogEndpoint was not set."),
       traceUrl = builder.Configuration.GetValue<string>("Telemetry:TraceEndpoint") 
                  ?? throw new ArgumentException("Telemetry.TraceEndpoint was not set.");


OtlpExporterOptions OtlpExporterOptsGen(string url)
{
    return new()
    {
        Endpoint = new Uri(url),
        Protocol = OtlpExportProtocol.HttpProtobuf
    };
}

builder
    .AddOpenTelemetry()
    .ConfigureResource(x =>
    {
        x.AddService(serviceName: builder.Environment.ApplicationName);
    })
    .WithTracing(opt =>
    {
        opt.AddAspNetCoreInstrumentation()
           .AddEntityFrameworkCoreInstrumentation()
           .AddOtlpExporter((_) => OtlpExporterOptsGen(traceUrl));
    });

builder.Logging.AddOpenTelemetry(opt =>
{
    opt.IncludeFormattedMessage = true;
    opt.IncludeScopes = true;
    opt.AddOtlpExporter((_) =>OtlpExporterOptsGen(logUrl));
    
});

builder.Services.AddDbContextFactory<ApplicationContext<RacingUserIdentity>>(opt =>
{
    opt.UseNpgsql(dbString);
});

// LOCAL SERVICES
builder.Services.AddScoped<IVehicleService, DbVehicleService>();
builder.Services.AddScoped<ITrackService, DbTrackService>();
builder.Services.AddScoped<IRacingClassService, DbRacingClassService>();
builder.Services.AddScoped<ILeagueService, DbLeagueService>();
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