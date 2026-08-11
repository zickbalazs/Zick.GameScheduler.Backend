using Microsoft.Extensions.Hosting;
using Quartz;
using Quartz.AspNetCore;

namespace Zick.GameScheduler.Backend.Scheduler
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            var builder = new HostApplicationBuilder(args);

            builder.Services.AddQuartz(quartz =>
            {
                quartz.UsePersistentStore(o =>
                {
                    o.UseSystemTextJsonSerializer();
                    o.UseProperties = true;
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