using System.Runtime.InteropServices;
using Docker.DotNet;
using Microsoft.EntityFrameworkCore;
using OpenTelemetry.Exporter;
using OpenTelemetry.Logs;
using OpenTelemetry.Resources;
using Prometheus;
using Quartz;
using Quartz.AspNetCore;
using Zick.GameScheduler.Backend.Data;
using Zick.GameScheduler.Backend.Data.Models;
using Zick.GameScheduler.Backend.Scheduler.Services;
using Zick.GameScheduler.Services.Generators;
using Zick.GameScheduler.SessionWorker;

var builder = Host.CreateApplicationBuilder(args);

string swarmUrl   = builder.Configuration.GetValue<string>("Docker:SwarmUrl")
                    ?? throw new ArgumentException("Docker Swarm's url was not set."),
       logsUrl    = builder.Configuration.GetValue<string>("Telemetry:LogsEndpoint") 
                    ?? throw new ArgumentException("Endpoint for logs must be set.");

int batchLimit = builder.Configuration.GetValue<int?>("SessionProcessor:BatchLimit")
                 ?? throw new ArgumentException("Batch limit was not set.");

builder.Services.AddLogging(opt =>
{
    opt.AddOpenTelemetry(x =>
    {
        x.IncludeScopes = true;
        x.IncludeFormattedMessage = true;
    });
});

builder
    .AddOpenTelemetry()
    .ConfigureResource(x =>
    {
        x.AddService(serviceName: builder.Environment.ApplicationName);
    })
    .WithLogging(x =>
    {
        x.AddOtlpExporter(opt =>
        {
            opt.Endpoint = new Uri(logsUrl);
            opt.Protocol = OtlpExportProtocol.HttpProtobuf;
        });
    });

Counter cancelledSessionMetric = 
    Metrics.CreateCounter("sessions_cancelled_total", "Total sessions that were cancelled");
Counter scheduledSessionMetric =
    Metrics.CreateCounter("sessions_scheduled_total", "Total sessions that were scheduled for start");
Counter lateSessionMetric =
    Metrics.CreateCounter("sessions_late_total", "Total sessions that were late but within the acceptable delta");

builder.Services.AddDbContextFactory<ApplicationContext<RacingUserIdentity>>(opt =>
{
    if (!RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
        opt.UseSqlite($"Data Source={Path.Join(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "com.zick.gs", "race.db")}");

    else
        opt.UseNpgsql(Environment.GetEnvironmentVariable("RACE_DB"));
});

builder.Services.AddScoped<IDockerClient>((_) => new DockerClientConfiguration(new Uri(swarmUrl)).CreateClient());

builder.Services.AddScoped<ApplicationContext<RacingUserIdentity>>(x =>
    x.GetRequiredService<IDbContextFactory<ApplicationContext<RacingUserIdentity>>>().CreateDbContext());

builder.Services.AddScoped<ISessionStatusUpdateService, DbSessionStatusUpdateService>();
builder.Services.AddScoped<IEntryGenerator<RacingUserIdentity>, DbEntryGenerator>();
builder.Services.AddScoped<IPortClaimService, DbPortClaimService>();
builder.Services.AddScoped<IContainerService, DockerSwarmContainerService>();






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
builder.Services.AddQuartzServer();

cancelledSessionMetric.Publish();
lateSessionMetric.Publish();
scheduledSessionMetric.Publish();
using var metricServer = new KestrelMetricServer(port: 1100, url: "/metrics");
metricServer.Start();

var host = builder.Build();
host.Run();