using Harpocrates.Runtime.Common.Configuration;
using Harpocrates.SecretManagement.Contracts.Data;
using Microsoft.Azure.Management.Graph.RBAC.Fluent;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.Azure.Management.Fluent;
using Microsoft.Azure.Management.ResourceManager.Fluent;
using Microsoft.Azure.Management.ResourceManager.Fluent.Authentication;
using Microsoft.Azure.Management.ResourceManager.Fluent.Core;
using System.Runtime.InteropServices.WindowsRuntime;

namespace Harpocrates.SecretManagement.Providers.Azure
{
    internal class AppRegistrationPasswordSecretManager : AzureSecretManager
    {
        public AppRegistrationPasswordSecretManager(IConfigurationManager config, ILogger logger) : base(config, logger)
        {
        }

        protected override async Task<Key> OnRotateSecretAsync(Secret secret, CancellationToken token)
        {
            //https://github.com/Azure-Samples/aad-dotnet-manage-service-principals/blob/master/Program.cs

            var auth = GetAzureAuthenticated();

            string accountId = $"";

            // var account = auth.ServicePrincipals.GetById("").Update().WithoutCredential("").WithPa

            var foo = auth.ActiveDirectoryApplications.GetById("").Update()
                .DefinePasswordCredential("")
                .WithPasswordValue("")
                .WithDuration(TimeSpan.FromSeconds(100))
                .Attach()
                .Apply();

            //if (account.TryGetValue("", out IPasswordCredential pwd))
            //{
            //    pwd.v
            //}


            throw new NotImplementedException();
        }

        private async Task<string> GenerateKey(Secret secret, string newKeyName, CancellationToken token)
        {
            Runtime.Common.DataAccess.ConnectionStrings.ServicePrincipalConnectionString sacs = new Runtime.Common.DataAccess.ConnectionStrings.ServicePrincipalConnectionString()
            {
                ConnectionString = secret.Configuration.SourceConnectionString
            };

            var auth = GetAzureAuthenticated();

            string appId = sacs.ClientId; // applicationId???

            var app = await auth.ActiveDirectoryApplications.GetByIdAsync(appId);

            string pwdName = $"Harpocrates Password {app.PasswordCredentials.Count + 1}";
            string pwd = GenerateStrongPassword();

            TimeSpan duration = TimeSpan.FromDays(365);
            if (secret.Configuration?.Policy != null) //should always be true if we got this far
                duration = secret.Configuration.Policy.RotationInterval + TimeSpan.FromHours(1); //add an hr to 'outlive' policy rotation interval

            var foo = await app.Update().DefinePasswordCredential(pwdName).WithPasswordValue(pwd).WithDuration(duration).Attach().ApplyAsync();
            return pwd;
        }

        private string GenerateStrongPassword()
        {
            return Runtime.Common.Utilities.PasswordGenerator.GeneratePassword(32);
        }
    }
}
