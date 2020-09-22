using Microsoft.Extensions.Configuration;

namespace Harpocrates.Runtime.Common.Configuration.KeyVault
{
    public class KeyVaultProxyConfigurationSource : IConfigurationSource
    {
        private readonly IConfigurationRoot _prebuiltConfig;
        private readonly ConfigurationOptions _options;
        public KeyVaultProxyConfigurationSource(IConfigurationRoot preBuiltConfig, ConfigurationOptions options)
        {
            _options = options;
            _prebuiltConfig = preBuiltConfig;
        }
        public IConfigurationProvider Build(IConfigurationBuilder builder)
        {
            return new KeyVaultProxyConfigurationProvider(_prebuiltConfig, _options);
        }
    }
}
