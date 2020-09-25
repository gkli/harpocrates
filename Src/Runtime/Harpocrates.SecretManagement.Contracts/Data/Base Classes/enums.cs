using System;
using System.Collections.Generic;
using System.Text;

namespace Harpocrates.SecretManagement.Contracts.Data
{
    public enum ServiceType
    {
        Unspecified,
        StorageAccountKey,
        CosmosDbAccountKey,
        CosmosDbAccountReadOnlyKey,
        SqlServerPassword,
        EventGrid,
        APIMManagement,
        ServicePrincipalSecret,
        RedisCache
    }

    public enum SecretType
    {
        ManagedSystem,
        Dependency
    }
}
