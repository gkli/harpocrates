
using System.Collections.Generic;

namespace Harpocrates.Runtime.Common.Configuration.KeyVault
{
    public class ConfigurationOptions
    {
        private readonly List<KeyVaultConfig> _configs = new List<KeyVaultConfig>();
        private readonly Dictionary<string, KeyVaultConfig> _index = new Dictionary<string, KeyVaultConfig>();

        private KeyVaultConfig _defaultConfig = null;
        public ConfigurationOptions() { }


        public List<KeyVaultConfig> KeyVaults { get { return _configs; } }
        public KeyVaultConfig DefaultConfig
        {
            get { return _defaultConfig; }
            set
            {
                _defaultConfig = value;
                if (!KeyVaults.Contains(_defaultConfig)) KeyVaults.Add(_defaultConfig);
            }
        }

        internal void Index()
        {
            _index.Clear();

            foreach (var cfg in _configs)
            {
                if (false == _index.ContainsKey(cfg.KeyVaultName.ToLower()))
                {
                    _index.Add(cfg.KeyVaultName.ToLower(), cfg);
                }
            }
        }

        internal KeyVaultConfig Get(string keyVaultName)
        {
            if (string.IsNullOrWhiteSpace(keyVaultName)) return null;

            keyVaultName = keyVaultName.ToLower();

            if (_index.ContainsKey(keyVaultName)) return _index[keyVaultName];

            return null;
        }
    }
}
