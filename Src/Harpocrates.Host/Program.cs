using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;

namespace Harpocrates.Host
{
    class Program
    {
        private static System.Threading.CancellationTokenSource _cts = new System.Threading.CancellationTokenSource();

        static void Main(string[] args)
        {
            var serviceCollection = new ServiceCollection();
            ConfigureServices(serviceCollection, args);

            var serviceProvider = serviceCollection.BuildServiceProvider();

            var host = serviceProvider.GetService<Runtime.Host>();

            host.StartAsync(_cts.Token).Wait();

        }


        private static void ConfigureServices(IServiceCollection services, string[] args)
        {

            IConfiguration config = new ConfigurationBuilder()
                    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                    .AddEnvironmentVariables()
                    .AddCommandLine(args)
                    .Build();

            services.AddLogging(configure => configure.AddConsole())
                //.Configure<LoggerFilterOptions>(options => options.MinLevel = LogLevel.Information)
                .AddSingleton<IConfiguration>(config)
                .AddSingleton<Runtime.Common.Configuration.IConfigurationManager, HostConfigurationManager>()
                //.AddTransient<Processors.StorageCalculator.DataAccess.IStorageDataAccessProvider, Processors.StorageCalculator.DataAccess.StorageAccount.StorageAccountDataAccessProvider>()
                //.AddTransient<Processors.StorageCalculator.DataAccess.IRecordKeepingDataAccessProvider, Processors.StorageCalculator.DataAccess.CosmosDb.CosmosDbRecordKeepingDataAccessProvider>()
                .AddTransient<Runtime.Host>();
        }
    }
}
