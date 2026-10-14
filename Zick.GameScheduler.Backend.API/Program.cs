using Microsoft.EntityFrameworkCore;
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


// TODO List:
// Upon starting for the first time, fill out the following tables:
// - Tracks
// - Vehicles
var builder = WebApplication.CreateBuilder(args);
builder.Logging.AddSimpleConsole();

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
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
            builder.Configuration.GetValue<string>("Cors:BackendURL"),
            builder.Configuration.GetValue<string>("Cors:FrontendURL")
        ]);
        m.WithMethods(["GET", "POST", "PATCH"]);
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
app.MapHub<RegistrationHub>("/hubs/reg");
app.MapControllers();

app.UseHttpsRedirection();

app.Run();