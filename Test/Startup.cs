using Hangfire.Dashboard;
using Hangfire.MemoryStorage;
using Hangfire.MissionControl;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Collections.Generic;

namespace Hangfire.PowerShellExecutor.Test
{
    public class Startup
    {
        /// <summary>
        /// This method gets called by the runtime. Use this method to add services to the container.
        /// </summary>
        /// <param name="services"></param>
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddHangfire(config =>
            {
                config.UseMemoryStorage();                                          // Configure job storage
                config.UseMissionControl(                                           // Configure MissionContol to be able to run tests easily
                    new MissionControlOptions { RequireConfirmation = false },
                    typeof(Startup).Assembly);
            });
        }

        /// <summary>
        /// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        /// </summary>
        /// <param name="app"></param>
        /// <param name="env"></param>
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();                                              // Add middleware for redirecting to HTTPS
            app.UseRouting();                                                       // Add middleware for routing

            app.UseHangfireDashboard(                                               // Add hangfire dashboard
                "",                                                                 // Application path
                new DashboardOptions
                {
                    DisplayStorageConnectionString = false,                         // Hide storage connection string
                    Authorization = new List<IDashboardAuthorizationFilter>(),      // Use default authorization filter
                    DashboardTitle = "PowerShell Executor"                          // Dashboard title
                });
            app.UseHangfireServer();                                                // Enable hangfire server
        }
    }
}