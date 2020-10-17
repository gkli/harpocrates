using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Harpocrates.Api.Host
{
    public class Startup
    {
        private static System.Threading.CancellationTokenSource _cts = new System.Threading.CancellationTokenSource();

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddApplicationInsightsTelemetry(Configuration["ApplicationInsights:InstrumentationKey"]);

            services.AddControllers();

            services.AddLogging(configure => configure.AddConsole())
                //.Configure<LoggerFilterOptions>(options => options.MinLevel = LogLevel.Information)
                .AddSingleton<Runtime.Common.Configuration.IConfigurationManager, ApiHostConfigurationManager>()
                .AddTransient<SecretManagement.DataAccess.ISecretMetadataDataAccessProvider>(s =>
                {
                    Runtime.Common.Configuration.IConfigurationManager cfg = s.GetRequiredService<Runtime.Common.Configuration.IConfigurationManager>();

                    return new SecretManagement.DataAccess.StorageAccount.SecretMetadataStorageAccountDataAccessProvider(
                        cfg.SecretManagementConnectionString, cfg);
                })
                .AddDbContext<Runtime.Tracking.SqlServer.Ef.TrackingDbContext>(options =>  // could turn into anon method, call options.GetRequiedService to get config, for now, we'll just short circuit
                    options.UseSqlServer(Configuration["Tracking:ConnectionString"])
                )
                .AddScoped<Runtime.Common.Tracking.IProcessingTrackerDataAccessProvider>(s =>
                {
                    //todo: construct DataAccessProvider here
                    //Runtime.Common.Configuration.IConfigurationManager cfg = s.GetRequiredService<Runtime.Common.Configuration.IConfigurationManager>();

                    return new Runtime.Tracking.SqlServer.SqlServerProcessingTrackerDataAccessProvider(s.GetRequiredService<Runtime.Tracking.SqlServer.Ef.TrackingDbContext>());
                    //return new Runtime.Common.Tracking.ConsoleProcessingTrackerDataAccessProvider();
                })
                .AddScoped<Runtime.Common.Tracking.IProcessingTracker, Runtime.Common.Tracking.ProcessingTracker>()
                .AddScoped<Runtime.Host>()
                .AddHostedService<BgWorker.ProcessingHostBackgroundService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

        }
    }
}
