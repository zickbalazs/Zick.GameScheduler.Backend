using Microsoft.EntityFrameworkCore;
using OpenTelemetry.Exporter;
using OpenTelemetry.Logs;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Zick.GameScheduler.Backend.API.Hubs;
using Zick.GameScheduler.Backend.Data;
using Zick.GameScheduler.Backend.Data.Models;
using Zick.GameScheduler.Services.API;
using Zick.GameScheduler.Services.ServicesAPI;

var builder = WebApplication.CreateBuilder(args);

// Pre-checks for variables
string backendUrl       = builder.Configuration.GetValue<string>("Cors:BackendURL") ??
                          throw new ArgumentException("Backend url must be set"),
       frontendUrl      = builder.Configuration.GetValue<string>("Cors:FrontendURL") ??
                          throw new ArgumentException("Frontend url must be set"),
       logsUrl          = builder.Configuration.GetValue<string>("OpenTelemetry:LogsEndpoint") ??
                          throw new ArgumentException("OpenTelemetry endpoint was not set."),
       tracesUrl        = builder.Configuration.GetValue<string>("OpenTelemetry:TracesEndpoint") ??
                          throw new ArgumentException("OpenTelemetry Traces were not set."),
       gameConnString   = builder.Configuration.GetConnectionString("GameDatabase") ??
                          throw new ArgumentException("Game DB was not given");

builder.AddOpenTelemetry()
    .ConfigureResource(x =>
    {
        x.AddService(serviceName: builder.Environment.ApplicationName);
    })
    .WithTracing(x=>x
        .AddAspNetCoreInstrumentation()
        .AddEntityFrameworkCoreInstrumentation()
        .AddOtlpExporter(options =>
        {
            options.Endpoint = new Uri(tracesUrl);
            options.Protocol = OtlpExportProtocol.HttpProtobuf;
        }));



builder.Logging.AddOpenTelemetry(opt => {
    opt.AddOtlpExporter(x =>
    {
        x.Protocol = OtlpExportProtocol.HttpProtobuf;
        x.Endpoint = new Uri(logsUrl);
    });
    opt.IncludeFormattedMessage = true;
    opt.IncludeScopes = true;
});

// Add services to the container.
builder.Services.AddOpenApi();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddAuthentication();
builder.Services.AddAuthorization();
builder.Services.AddControllers();



builder.Services.AddCors(x =>
{
    x.AddDefaultPolicy(m =>
    {
        m.WithOrigins([
            backendUrl,
            frontendUrl
        ]);
        m.SetIsOriginAllowed((host) => true);
        m.WithMethods(["GET", "POST", "PATCH", "DELETE"]);
        m.WithHeaders(["Content-Type"]);
        m.AllowCredentials();
    });
});

builder.Services.AddDbContext<ApplicationContext<RacingUserIdentity>>(opt =>
{
    opt.UseNpgsql(gameConnString);
});

// DATA OBJECTS
builder.Services.AddScoped<IVehicleService, DbVehicleService>();
builder.Services.AddScoped<ISessionService, DbSessionService>();
builder.Services.AddScoped<ILeagueService, DbLeagueService>();
builder.Services.AddScoped<IResultService, DbResultService>();
builder.Services.AddScoped<ITrackService, DbTrackService>();
builder.Services.AddScoped<IRegistrationService, DbRegistrationService>();
// AUTH
builder.Services.AddIdentityApiEndpoints<RacingUserIdentity>()
    .AddEntityFrameworkStores<ApplicationContext<RacingUserIdentity>>();

// SIGNALR
builder.Services.AddSignalR();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapSwagger();
    app.MapSwaggerUI();
}

app.UseCors();
app.MapIdentityApi<RacingUserIdentity>();
app.MapControllers();
app.UseAuthentication();
app.UseAuthorization();
app.MapHub<RegistrationHub>("/hub/sessions");
app.UseHttpsRedirection();

app.Run();