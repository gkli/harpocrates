using System;
using Harpocrates.Runtime.Common.Contracts;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;

namespace Harpocrates.Host.AzureFunction
{
    public static class RawMessageHandlerFunction
    {
        [FunctionName("RawMessageHandler")]
        public static void Run([QueueTrigger("events", Connection = "EventQueueConnectionString")] string myQueueItem, ILogger log)
        {
            log.LogInformation($"C# Queue trigger function processed: {myQueueItem}");

            Runtime.Common.Configuration.ConfigurationManager config = new FunctionConfigurationManager();
            Azure.Storage.Queues.QueueClient c = Runtime.Helpers.QueueClientHelper.CreateQueueClient(config, config.RawMessagesQueueName);
            Azure.Storage.Queues.QueueClient d = Runtime.Helpers.QueueClientHelper.CreateQueueClient(config, config.DeadLetterMessagesQueueName);

            Runtime.MessageHandler<RawProcessRequest> handler = new Runtime.MessageHandler<RawProcessRequest>(c, d, config, log);

            var result = handler.ProcessMessageAsync(myQueueItem, new System.Threading.CancellationTokenSource().Token).Result;

            log.LogInformation($"Request processed. Status:{result.Status}");
        }
    }
}
