using Harpocrates.Runtime.Common.Configuration.KeyVault;
using Microsoft.Azure.Management.Monitor.Fluent.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using Microsoft.Extensions.Logging.ApplicationInsights;

namespace Harpocrates.Host
{
    class Program
    {
        private static System.Threading.CancellationTokenSource _cts = new System.Threading.CancellationTokenSource();

        static void Main(string[] args)
        {
            string appInsightsKey = "";

            var builder = new HostBuilder()
                .ConfigureAppConfiguration((context, config) =>
            {

                var builtConfig = config.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                  .AddEnvironmentVariables()
                  .AddCommandLine(args)
                  .Build();

                appInsightsKey = builtConfig["ApplicationInsights:InstrumentationKey"];

                ManagedIdentityKeyVaultConfig cfg = new ManagedIdentityKeyVaultConfig(builtConfig["KVConfig:KeyVaultName"]);


                ConfigurationOptions options = new ConfigurationOptions();
                options.DefaultConfig = cfg;

                config.AddKeyVaultProxyConfigurationProvider(builtConfig, options);

                //context.Configuration = config.Build();

            }).ConfigureServices((context, services) =>
            {
                //todo: read from config
                services.AddLogging(configure => configure.AddConsole().AddApplicationInsights(appInsightsKey))
                    //.Configure<LoggerFilterOptions>(options => options.MinLevel = LogLevel.Information)
                    .AddSingleton<IConfiguration>(context.Configuration)
                    .AddSingleton<Runtime.Common.Configuration.IConfigurationManager, HostConfigurationManager>()
                    .AddTransient<SecretManagement.DataAccess.ISecretMetadataDataAccessProvider>(s =>
                    {
                        Runtime.Common.Configuration.IConfigurationManager cfg = s.GetRequiredService<Runtime.Common.Configuration.IConfigurationManager>();

                        return new SecretManagement.DataAccess.StorageAccount.SecretMetadataStorageAccountDataAccessProvider(
                            cfg.SecretManagementConnectionString, cfg);
                    })
                    //.AddApplicationInsightsTelemetryWorkerService("")
                    .AddTransient<Runtime.Host>();
            });

            var appHost = builder.Build();

            var host = appHost.Services.GetService<Runtime.Host>();

            //host.CreateSampleDataSetAsync(_cts.Token, true, false, false).Wait();

            host.StartAsync(_cts.Token).Wait();
        }

    }
}
