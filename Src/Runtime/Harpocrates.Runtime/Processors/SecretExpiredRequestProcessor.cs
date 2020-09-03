using Harpocrates.Runtime.Common.Contracts;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Harpocrates.Runtime.Processors
{
    class SecretExpiredRequestProcessor : RequestProcessor<FormattedProcessRequest>
    {
        public SecretExpiredRequestProcessor(Common.Configuration.IConfigurationManager config, ILogger logger) : base(config, logger)
        {
        }

        protected override async Task OnProcessRequestAsync(FormattedProcessRequest request, ProcessResult result, CancellationToken token)
        {
            SecretManagement.ISecretMetadataManager manager = new SecretManagement.SecretMetadataManager(Config, Logger);

            try
            {
                await manager.ProcessExpiredSecretAsync(request.ObjectUri, token);

                if (null != token && token.IsCancellationRequested)
                {
                    result.Status = ProcessResult.ProcessingStatus.Failed | ProcessResult.ProcessingStatus.Aborted;
                    return;
                }
                else
                    result.Status |= ProcessResult.ProcessingStatus.Success;
            }
            //catch (Workers.Processors.StorageCalculator.Exceptions.SizeCalculationFailedException scfe)
            //{
            //    result.Status |= ProcessResult.ProcessingStatus.Failed;
            //    result.Description = scfe.Message;
            //}
            //catch (Workers.Processors.StorageCalculator.Exceptions.SizeUpdatedFailedException sufe)
            //{
            //    result.Status |= ProcessResult.ProcessingStatus.Failed;
            //    result.Description = sufe.Message;
            //}
            catch (Exception ex)
            {
                result.Status |= ProcessResult.ProcessingStatus.Failed;
                result.Description = $"Unexpected exception occured. Vault Name: {request.VaultName}. Object Name: {request.ObjectName}. Object Type: {request.ObjectType} Error: {ex.Message}";
            }

        }
    }
}
