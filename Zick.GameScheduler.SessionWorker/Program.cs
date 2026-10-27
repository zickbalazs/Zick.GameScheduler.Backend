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

string logsUrl = builder.Configuration.GetValue<string>("Telemetry:LogsEndpoint") 
                    ?? throw new ArgumentException("Endpoint for logs must be set."),
       gameDb  = builder.Configuration.GetConnectionString("GameDatabase")
                 ?? throw new ArgumentException("GameDatabase connection was not set."),
       schedDb = builder.Configuration.GetConnectionString("SchedulerDatabase")
                 ?? throw new ArgumentException("SchedulerDatabase connection was not set.");

int batchLimit = builder.Configuration.GetValue<int?>("SessionProcessor:BatchLimit")
                 ?? throw new ArgumentException("Batch limit was not set.");

builder.Services.AddLogging(opt =>
{
    opt.AddOpenTelemetry(x =>
    {
        x.IncludeScopes = true;
        x.IncludeFormattedMessage = true;
        x.AddOtlpExporter(opt =>
        {
            opt.Endpoint = new Uri(logsUrl);
            opt.Protocol = OtlpExportProtocol.HttpProtobuf;
        });
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
    opt.UseNpgsql(gameDb);
});

builder.Services.AddHostedService<SessionProcessorWorker>();
builder.Services.AddQuartz(opt =>
{
    opt.UsePersistentStore(o =>
    {
        o.UseSystemTextJsonSerializer();
        o.UseProperties = true;
        o.UsePostgres(schedDb);
    });
});

cancelledSessionMetric.Publish();
lateSessionMetric.Publish();
scheduledSessionMetric.Publish();
using var metricServer = new KestrelMetricServer(port: 1100, url: "/metrics");
metricServer.Start();

var host = builder.Build();
host.Run();