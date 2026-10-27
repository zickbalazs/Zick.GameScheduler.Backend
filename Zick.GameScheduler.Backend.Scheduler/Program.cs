using Docker.DotNet;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using OpenTelemetry.Exporter;
using OpenTelemetry.Logs;
using Quartz;
using Quartz.AspNetCore;
using Zick.GameScheduler.Backend.Data;
using Zick.GameScheduler.Backend.Data.Models;
using Zick.GameScheduler.Backend.Scheduler.Services;
using Zick.GameScheduler.Services.Generators;

var builder = Host.CreateApplicationBuilder(args);

string gameDb =   builder.Configuration.GetConnectionString("GameDatabase")
                  ?? throw new ArgumentException("GameDatabase connection was not set."),
    schedulerDb = builder.Configuration.GetConnectionString("SchedulerDatabase")
                  ?? throw new ArgumentException("SchedulerDatabase connection was not set."),
    swarmUrl    = builder.Configuration.GetValue<string>("Docker:SwarmUrl")
                  ?? throw new ArgumentException("Docker.SwarmUrl was not set."),
    logsUrl     = builder.Configuration.GetValue<string>("Telemetry:LogsUrl")
                  ?? throw new ArgumentException("Telemetry.LogsUrl was not set.");

builder.Logging.AddOpenTelemetry(opt =>
{
    opt.IncludeFormattedMessage = true;
    opt.IncludeScopes = true;
    opt.AddOtlpExporter(x =>
    {
        x.Endpoint = new Uri(logsUrl);
        x.Protocol = OtlpExportProtocol.HttpProtobuf;
    });
});
builder.Services.AddOpenTelemetry();


builder.Services.AddQuartz(opt =>
{
    opt.UsePersistentStore(x =>
    {
        x.PerformSchemaValidation = true;
        x.UseProperties = true;
        x.UseSystemTextJsonSerializer();
        x.UsePostgres(schedulerDb);
        x.UseClustering();
    });
});

builder.Services.AddDbContextFactory<ApplicationContext<RacingUserIdentity>>(opt =>
{
    opt.UseNpgsql(gameDb);
});
builder.Services.AddScoped<ApplicationContext<RacingUserIdentity>>(x =>
    x.GetRequiredService<IDbContextFactory<ApplicationContext<RacingUserIdentity>>>().CreateDbContext());


builder.Services.AddScoped<IDockerClient>(_ => new DockerClientConfiguration(new Uri(swarmUrl)).CreateClient());
builder.Services.AddScoped<IPortClaimService, DbPortClaimService>();
builder.Services.AddScoped<IEntryGenerator<RacingUserIdentity>, DbEntryGenerator>();
builder.Services.AddScoped<ISessionStatusUpdateService, DbSessionStatusUpdateService>();
builder.Services.AddScoped<IContainerService, DockerSwarmContainerService>();






builder.Services.AddQuartzServer(o =>
{
    o.WaitForJobsToComplete = true;
});

var app = builder.Build();
app.Run();