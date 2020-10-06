using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Harpocrates.Runtime.Common.Configuration.KeyVault;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Harpocrates.Api.Host
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Microsoft.Extensions.Hosting.Host.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration((context, config) =>
                {
                    var builtConfig = config.Build();
                    ManagedIdentityKeyVaultConfig cfg = new ManagedIdentityKeyVaultConfig(builtConfig["KVConfig:KeyVaultName"]);

                    ConfigurationOptions options = new ConfigurationOptions();
                    options.DefaultConfig = cfg;

                    config.AddKeyVaultProxyConfigurationProvider(builtConfig, options);

                })
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
