using Microsoft.EntityFrameworkCore;
using OpenTelemetry.Exporter;
using OpenTelemetry.Logs;
using Zick.GameScheduler.Backend.API.Hubs;
using Zick.GameScheduler.Backend.Data;
using Zick.GameScheduler.Backend.Data.Models;
using Zick.GameScheduler.Services.API;
using Zick.GameScheduler.Services.ServicesAPI;

// TODO List:
// - Services for proper frontend functions [ ]
// - SignalR for real time registration data [ ]
// - Auth for requests that require it [ ]
// - Caching for cacheable requests (vehicles, tracks, etc.) [ ]
// - Proper error templates

var builder = WebApplication.CreateBuilder(args);
builder.Logging.AddSystemdConsole(x =>
{
    x.TimestampFormat = "[%y-%M-%d: %h:%m:%s]: ";
});
builder.Logging.AddOpenTelemetry(opt => {
        opt.AddOtlpExporter(x =>
        {
            x.Protocol = OtlpExportProtocol.Grpc;
        });
        opt.IncludeFormattedMessage = true;
        opt.IncludeScopes = true;
});
builder.AddOpenTelemetry();

// Add services to the container.
builder.Services.AddOpenApi();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddAuthentication();
builder.Services.AddAuthorization();
builder.Services.AddControllers();


string backendUrl = builder.Configuration.GetValue<string>("Cors:BackendURL") ?? throw new ArgumentException("Backend url must be set"),
       frontendUrl = builder.Configuration.GetValue<string>("Cors:FrontendURL") ?? throw new ArgumentException("Frontend url must be set") ;

builder.Services.AddCors(x =>
{
    x.AddDefaultPolicy(m =>
    {
        m.WithOrigins([
            backendUrl,
            frontendUrl
        ]);
        m.WithMethods(["GET", "POST", "PATCH"]);
        m.WithHeaders(["Content-Type"]);
        m.AllowCredentials();
    });
});


if (builder.Environment.IsDevelopment())
{
    builder.Services.AddDbContext<ApplicationContext<RacingUserIdentity>>(opt =>
    {
        opt.UseSqlite($"Data Source={Path.Join(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "com.zick.gs", "race.db")}");
    });
}
else
{
    builder.Services.AddDbContext<ApplicationContext<RacingUserIdentity>>(opt =>
    {
        opt.UseNpgsql(Environment.GetEnvironmentVariable("RACE_DB"));
    });
}

// DATA OBJECTS
builder.Services.AddScoped<IVehicleService, DbVehicleService>();
builder.Services.AddScoped<ISessionService, DbSessionService>();
builder.Services.AddScoped<ILeagueService, DbLeagueService>();
builder.Services.AddScoped<IResultService, DbResultService>();
builder.Services.AddScoped<ITrackService, DbTrackService>();

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
app.MapHub<RegistrationHub>("/hub/sessions");
app.UseHttpsRedirection();

app.Run();