using Harpocrates.Runtime.Common.Contracts;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Harpocrates.Runtime.Processors
{
    public class RawRequestProcessor : RequestProcessor<RawProcessRequest>
    {
        public RawRequestProcessor(Common.Configuration.IConfigurationManager config, Common.Tracking.IProcessingTracker tracker, ILogger logger) : base(config, tracker, logger) { }

        protected override async Task OnProcessRequestAsync(RawProcessRequest request, ProcessResult result, CancellationToken token)
        {

            FormattedProcessRequest fr = request.FormatRequest();

            try
            {

                if (fr.Action == FormattedProcessRequest.RequestedAction.DoNothing)
                {
                    result.Status |= ProcessResult.ProcessingStatus.Skipped;
                    Logger.LogInformation($"Event processing is being skipped. Vault Name: {fr.VaultName}. Object: {fr.ObjectName}.");
                }
                else
                {
                    //THIS NEEDS TO CHANGE TO USE SHARED CLIENT to avoid Socket exceptions
                    await Helpers.QueueClientHelper.CreateQueueClient(Config, Config.FormattedMessagesQueueName).SendMessageAsync(fr.Serialize());
                }
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
