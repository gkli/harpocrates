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
        AppRegistrationPassword,
        // AppRegistrationCertificate ???
        RedisCache
    }

    public enum SecretType
    {
        /// <summary>
        /// Secret that is 'attached' to a service that is managed by the application
        /// </summary>
        Attached,
        /// <summary>
        /// Secret that is not directly related to an 'attached' secret, but is related to other managed secret(s)
        /// </summary>
        Dependency
    }
}
