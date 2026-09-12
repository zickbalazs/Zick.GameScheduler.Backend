using System.Runtime.InteropServices;
using Microsoft.EntityFrameworkCore;
using MudBlazor.Services;
using Zick.GameScheduler.Backend.Dashboard.Components;
using Zick.GameScheduler.Backend.Dashboard.Services;
using Zick.GameScheduler.Backend.Data;
using Zick.GameScheduler.Backend.Data.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddDbContext<ApplicationContext<RacingUserIdentity>>(opt =>
{
    if (!RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
        opt.UseSqlite($"Data Source={Path.Join(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "com.zick.gs", "race.db")}");

    else
        opt.UseNpgsql(Environment.GetEnvironmentVariable("RACE_DB"));
});
    
builder.Services.AddScoped<IVehicleService, DbVehicleService>();
builder.Services.AddScoped<ITrackService, DbTrackService>();
builder.Services.AddScoped<IRacingClassService, DbRacingClassService>();

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