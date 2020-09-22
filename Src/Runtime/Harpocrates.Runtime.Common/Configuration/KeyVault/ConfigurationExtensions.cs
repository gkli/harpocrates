using Microsoft.Extensions.Configuration;

namespace Harpocrates.Runtime.Common.Configuration.KeyVault
{
    public static class ConfigurationExtensions
    {
        public static IConfigurationBuilder AddKeyVaultProxyConfigurationProvider(
            this IConfigurationBuilder configuration, IConfigurationRoot preBuiltConfig, ConfigurationOptions options)
        {
            configuration.Add(new KeyVaultProxyConfigurationSource(preBuiltConfig, options));
            return configuration;
        }
    }
}
