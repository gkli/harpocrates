using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Harpocrates.Api.Host.BgWorker
{
    public class ProcessingHostBackgroundService : BackgroundService
    {
        private readonly ILogger<ProcessingHostBackgroundService> _logger;

        public ProcessingHostBackgroundService(IServiceProvider services,
            ILogger<ProcessingHostBackgroundService> logger)
        {
            Services = services;
            _logger = logger;
        }

        public IServiceProvider Services { get; }


        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Consume Scoped Service Hosted Service is working.");
            
            using (var scope = Services.CreateScope())
            {
                var host =
                    scope.ServiceProvider
                        .GetRequiredService<Runtime.Host>();

                await host.StartAsync(stoppingToken);
            }
        }
    }
}
