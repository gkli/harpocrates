using Azure.Storage.Queues.Models;
using Harpocrates.Runtime.Common.Contracts;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Harpocrates.Runtime
{
    public class MessageHandler<T> where T : ProcessRequest
    {
        private readonly Azure.Storage.Queues.QueueClient _queueClient;
        private readonly Azure.Storage.Queues.QueueClient _deadLetterQueueClient;

        private readonly Common.Configuration.IConfigurationManager _config;

        private readonly ILogger _logger;
        private readonly RequestProcessorFactory _factory;

        public MessageHandler(Azure.Storage.Queues.QueueClient queueClient, Azure.Storage.Queues.QueueClient deadLetterClient, Common.Configuration.IConfigurationManager config, ILogger logger)
        {
            _queueClient = queueClient;
            _deadLetterQueueClient = deadLetterClient;

            _config = config;
            _logger = logger;

            _factory = new RequestProcessorFactory(_config, _logger);
        }


        public async Task<ProcessResult> ProcessMessageAsync(QueueMessage msg, CancellationToken token)
        {
            ProcessResult result = await ProcessMessageAsync(msg.MessageText, token);

            if (result != null)
            {
                if ((result.Status & (ProcessResult.ProcessingStatus.Failed | ProcessResult.ProcessingStatus.Skipped | ProcessResult.ProcessingStatus.DeadLetter)) > 0)
                {
                    //await _queueClient.DeleteMessageAsync(msg.MessageId, msg.PopReceipt, token); //
                    //return result;
                    result.Status = ProcessResult.ProcessingStatus.Failed | ProcessResult.ProcessingStatus.DeleteMessage; //force delete to occur - do not mark as deadletter as aloread moved to dead earlier
                }

                long attemptNumber = msg.DequeueCount;

                if ((result.Status & ProcessResult.ProcessingStatus.Skipped) > 0)
                {
                    _logger.LogInformation($"Message processing skipped. Queue: {_queueClient.Name}. MessageId: {msg.MessageId}.");
                }

                if ((result.Status & ProcessResult.ProcessingStatus.Success) > 0)
                {
                    _logger.LogInformation($"Message processed sucessfully. Queue: {_queueClient.Name}. MessageId: {msg.MessageId}.");
                    result.Status |= ProcessResult.ProcessingStatus.DeleteMessage; //message can be removed as it was processed...
                }
                else
                {
                    //did not succeed

                    if (attemptNumber > _config.MaxNumberProcessingAttempts) //since attempt is gather AFTER processing, use > instead of >=
                    {
                        result.Status |= ProcessResult.ProcessingStatus.DeadLetter;
                        result.Status &= ~ProcessResult.ProcessingStatus.RetryRequested; //cancel any request for retry as it is now destined for dead letter
                    }

                    //if marked for dead letter, we want to ensure it is removed from current queue
                    if ((result.Status & ProcessResult.ProcessingStatus.DeadLetter) > 0)
                    {
                        result.Status |= ProcessResult.ProcessingStatus.DeleteMessage; //messages that are moved to dead letter, need to be deleted
                    }
                    else
                    {
                        //retry the opeartion on all message not already marked for dead letter
                        result.Status |= ProcessResult.ProcessingStatus.RetryRequested;
                    }
                }

                if ((result.Status & ProcessResult.ProcessingStatus.RetryRequested) > 0)
                {
                    //if retrying, make sure there's no dead letter being requested...
                    result.Status &= ~ProcessResult.ProcessingStatus.DeadLetter; //should never really be needed

                    //either leave alone or put back on queue
                    //if (_config.RetryInline)
                    //{
                    //remove delete message flag...
                    _logger.LogWarning($"Message retry has been requested. Message processing will be attempted by next available worker. Queue: {_queueClient.Name}. MessageId: {msg.MessageId}. Attempt #: {attemptNumber}");
                    result.Status &= ~ProcessResult.ProcessingStatus.DeleteMessage;
                    //}
                    //else
                    //{
                    //    _logger.LogWarning($"Message retry has been requested. Placing message back on the queue. Queue: {_queueClient.Name}. MessageId: {msg.MessageId}.  Attempt #: {attemptNumber}");
                    //    await _queueClient.SendMessageAsync(request.Serialize(), token);
                    //    result.Status |= ProcessResult.ProcessingStatus.DeleteMessage; //ensure message is removed from queue
                    //}
                }


                if ((result.Status & ProcessResult.ProcessingStatus.DeadLetter) > 0)
                {
                    _logger.LogWarning($"Unable to process the message. Message is being archived. Queue: {_queueClient.Name}. MessageId: {msg.MessageId}. Error: {result.Description}");

                    await _deadLetterQueueClient.SendMessageAsync(msg.MessageText);

                    result.Status |= ProcessResult.ProcessingStatus.DeleteMessage; //ensure message is removed from queue
                }

                if ((result.Status & ProcessResult.ProcessingStatus.DeleteMessage) > 0)
                    await _queueClient.DeleteMessageAsync(msg.MessageId, msg.PopReceipt, token);

            }
            return result;
        }

        public async Task<ProcessResult> ProcessMessageAsync(string messageText, CancellationToken token)
        {

            T request = null;

            //raw is loaded directly where formatted is deserialized
            if (typeof(T) == typeof(RawProcessRequest) || typeof(RawProcessRequest).IsSubclassOf(typeof(T)))
            {
                request = new RawProcessRequest(messageText) as T;
            }
            else
            {
                try
                {
                    request = Newtonsoft.Json.JsonConvert.DeserializeObject<T>(messageText);
                }
                catch (Newtonsoft.Json.JsonReaderException)
                {
                    //determine error thrown when json is invalid or doesn't match...
                    request = null;
                }
            }


            if (null == request)
            {
                //put on dead letter queue and move on...
                //_logger.LogWarning($"Unable to deserialize the message. Message is being archived. MessageId: {msg.MessageId}");
                await _deadLetterQueueClient.SendMessageAsync(messageText);
                //await _queueClient.DeleteMessageAsync(msg.MessageId, msg.PopReceipt, token);
                return new ProcessResult()
                {
                    Status = ProcessResult.ProcessingStatus.Failed | ProcessResult.ProcessingStatus.Skipped | ProcessResult.ProcessingStatus.DeadLetter,
                    Description = "Unable to deserialize the message. Message is being archived."
                };
            }

            return await ProcessMessageAsync(request, token);

        }

        private async Task<ProcessResult> ProcessMessageAsync(ProcessRequest request, CancellationToken token)
        {
            return await _factory.ProcessRequestAsync(request, token);
        }
    }
}
