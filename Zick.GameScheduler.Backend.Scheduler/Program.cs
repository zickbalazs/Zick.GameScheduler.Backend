using System.Runtime.InteropServices;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Quartz;
using Quartz.AspNetCore;

namespace Zick.GameScheduler.Backend.Scheduler
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            var builder = new HostApplicationBuilder(args);

            builder.Services.AddOpenTelemetry();
            builder.Logging.AddOpenTelemetry();
            
            
            builder.Services.AddQuartz(quartz =>
            {
                quartz.UsePersistentStore(o =>
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

            builder.Services.AddQuartzServer(quartz =>
            {
                quartz.WaitForJobsToComplete = false;
            });

                        
            
            var app = builder.Build();
            
            app.Run();
        }    
    }
}