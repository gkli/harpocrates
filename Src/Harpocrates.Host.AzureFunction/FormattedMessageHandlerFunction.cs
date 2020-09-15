using Harpocrates.Runtime.Common.Contracts;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace Harpocrates.Host.AzureFunction
{
    public static class FormattedMessageHandlerFunction
    {
        [FunctionName("FormattedMessageHandler")]
        public static void Run([QueueTrigger("formatted", Connection = "EventQueueConnectionString")] string myQueueItem, ILogger log)
        {
            log.LogInformation($"C# Queue trigger function processed: {myQueueItem}");

            Runtime.Common.Configuration.ConfigurationManager config = new FunctionConfigurationManager();
            Azure.Storage.Queues.QueueClient c = Runtime.Helpers.QueueClientHelper.CreateQueueClient(config, config.FormattedMessagesQueueName);
            Azure.Storage.Queues.QueueClient d = Runtime.Helpers.QueueClientHelper.CreateQueueClient(config, config.DeadLetterMessagesQueueName);

            Runtime.MessageHandler<FormattedProcessRequest> handler = new Runtime.MessageHandler<FormattedProcessRequest>(c, d, config, log);

            var result = handler.ProcessMessageAsync(myQueueItem, new System.Threading.CancellationTokenSource().Token).Result;

            log.LogInformation($"Request processed. Status:{result.Status}");
        }

    }
}
