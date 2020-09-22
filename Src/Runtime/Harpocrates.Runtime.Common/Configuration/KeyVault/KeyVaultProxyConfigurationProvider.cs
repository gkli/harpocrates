using System;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using Microsoft.Azure.KeyVault;
using Microsoft.Azure.KeyVault.Models;


namespace Harpocrates.Runtime.Common.Configuration.KeyVault
{
    public class KeyVaultProxyConfigurationProvider : ConfigurationProvider
    {
        private readonly IConfigurationRoot _config;
        private readonly ConfigurationOptions _options;
        private readonly Dictionary<string, KeyVaultConfig> _vaultIndex = new Dictionary<string, KeyVaultConfig>();
        public KeyVaultProxyConfigurationProvider(IConfigurationRoot config, ConfigurationOptions options)
        {
            _config = config;
            _options = options;
        }

        public override void Load()
        {
            base.Load();

            _options.Index();

            ProcessConfigSection(_config.GetChildren());
        }

        private void ProcessConfigSection(IEnumerable<IConfigurationSection> sections)
        {
            if (null == sections) return;

            foreach (var section in sections)
            {
                if (section.Exists())
                {
                    //System.Diagnostics.Debug.WriteLine($"Key: {section.Key}, Path: {section.Path}, Value: {section.Value}");

                    if (false == string.IsNullOrWhiteSpace(section.Value) && section.Value.ToLower().StartsWith("vault://"))
                    {
                        if (Uri.TryCreate(section.Value, UriKind.Absolute, out Uri uri))
                        {
                            KeyVaultConfig kvconfig = null;
                            string host = uri.Host;
                            if (host == ".")
                            {
                                if (null == _options.DefaultConfig)
                                {
                                    throw new InvalidOperationException("Cannot use '.' without a valid DefaultConfig being set");
                                }

                                kvconfig = _options.DefaultConfig;

                            }
                            else
                            {
                                //find the config for specific hostname
                                kvconfig = _options.Get(host);
                            }

                            if (null == kvconfig)
                            {
                                throw new InvalidOperationException($"Unable to locate configuration for {host} keyvault");
                            }

                            host = $"https://{kvconfig.KeyVaultName}.vault.azure.net/";

                            string secretName = uri.Segments[1]; //syntax should be /secretname
                            if (false == string.IsNullOrWhiteSpace(secretName))
                            {
                                SecretBundle secretBundle = kvconfig.GetKeyVaultClient().GetSecretAsync(host, secretName).Result;

                                //or maybe replace it in the underlying provider?
                                if (Data.ContainsKey(section.Path))
                                {
                                    Data[section.Path] = secretBundle.Value;
                                }
                                else
                                {
                                    Data.Add(section.Path, secretBundle.Value);
                                }
                            }

                        }



                    }

                    ProcessConfigSection(section.GetChildren());
                }

            }

        }

    }
}
