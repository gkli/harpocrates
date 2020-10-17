using Harpocrates.Runtime.Common.Configuration;
using Harpocrates.Runtime.Common.Contracts;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Harpocrates.Runtime.Processors
{
    class ScheduleDependencyUpdatesRequestProcessor : RequestProcessor<FormattedProcessRequest>
    {
        public ScheduleDependencyUpdatesRequestProcessor(IConfigurationManager config, Common.Tracking.IProcessingTracker tracker, ILogger logger) : base(config, tracker, logger)
        {
        }

        protected override async Task OnProcessRequestAsync(FormattedProcessRequest request, ProcessResult result, CancellationToken token)
        {
            try
            {
                SecretManagement.DataAccess.ISecretMetadataDataAccessProvider dataProvider = Config.ServiceProvider.GetRequiredService<SecretManagement.DataAccess.ISecretMetadataDataAccessProvider>();

                string key = SecretManagement.Contracts.Data.SecretBase.FromKeyvaultUri(request.ObjectUri).Key;

                //var secret = await dataProvider.GetSecretAsync(key, token);
                //if (null != secret && secret.SecretType == SecretManagement.Contracts.Data.SecretType.Dependency)
                //    result.Status |= ProcessResult.ProcessingStatus.Skipped; // skip scheduling dependencies for
                //else
                //{
                var dependencies = await dataProvider.GetDependentSecretsAsync(key, token);
                List<Task> workers = new List<Task>();
                foreach (var dependency in dependencies)
                {
                    FormattedProcessRequest fpr = new FormattedProcessRequest(request.OriginalMessageJson, FormattedProcessRequest.RequestedAction.PerformDependencyUpdate)
                    {
                        ParentTransactionId = request.TransactionId,
                        Event = request.Event,
                        ObjectName = dependency.ObjectName,
                        SubscriptionId = dependency.SubscriptionId,
                        VaultName = dependency.VaultName,
                        ObjectUri = dependency.Uri,
                        ObjectType = FormattedProcessRequest.SecretType.Secret //todo: should this be parsed out?

                    };
                    workers.Add(Helpers.QueueClientHelper.CreateQueueClient(Config, Config.FormattedMessagesQueueName).SendMessageAsync(fpr.Serialize()));
                }

                Task.WaitAll(workers.ToArray());
                // }                

                result.Status |= ProcessResult.ProcessingStatus.Success;
            }
            catch (Exception ex) //todo: filter for specific exceptions...
            {
                result.Status |= ProcessResult.ProcessingStatus.Failed;
                result.Description = ex.Message;

            }
        }
    }
}
