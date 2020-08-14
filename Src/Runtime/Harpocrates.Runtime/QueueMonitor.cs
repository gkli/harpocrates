using Azure.Storage.Queues.Models;
using Harpocrates.Runtime.Common.Contracts;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Harpocrates.Runtime
{
    internal class QueueMonitor<T> where T : ProcessRequest
    {
        //private readonly string _storageAccountConnectionString;
        private Azure.Storage.Queues.QueueClient _queueClient;
        private Azure.Storage.Queues.QueueClient _deadLetterQueueClient;

        private ILogger _logger;
        private Common.Configuration.IConfigurationManager _config;
        private TimeSpan _messageVisibilityTimeout;

        public QueueMonitor(Uri queueUri, TimeSpan messageVisibilityTimeout, Common.Configuration.IConfigurationManager config, ILogger logger)
        {
            _logger = logger;
            _config = config;

            _queueClient = Helpers.QueueClientHelper.CreateQueueClient(queueUri, config);

            _deadLetterQueueClient = Helpers.QueueClientHelper.CreateQueueClient(config, config.DeadLetterMessagesQueueName);
            _messageVisibilityTimeout = messageVisibilityTimeout;
        }

        public async Task ProcessPendingMessagesAsync(CancellationToken token)
        {
            try
            {
                await EnsureQueueAsync();

                RequestProcessorFactory factory = new RequestProcessorFactory(_config, _logger);

                //currently have a permissions issue w/ accessing queue...
                //under "Queue Service" in storage account UI, make sure to set "Authentication Method" to "Azure AD user Account" instead of "Access Key"
                while (!token.IsCancellationRequested && (await PendingMessagesExistAsync(token)))
                {
                    QueueMessage[] messages = null;

                    try
                    {
                        messages = await _queueClient.ReceiveMessagesAsync(maxMessages: 1, visibilityTimeout: _messageVisibilityTimeout, cancellationToken: token);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError($"Unable to receive messages. Account: {_queueClient.AccountName} Queue: {_queueClient.Name} Exception: {ex.Message}");
                    }

                    if (null != messages && messages.Length > 0)
                    {
                        foreach (var msg in messages)
                        {
                            T request = null;

                            //raw is loaded directly where formatted is deserialized
                            if (typeof(T) == typeof(RawProcessRequest) || typeof(RawProcessRequest).IsSubclassOf(typeof(T)))
                            {
                                request = new RawProcessRequest(msg.MessageText) as T;
                            }
                            else
                            {
                                try
                                {
                                    request = Newtonsoft.Json.JsonConvert.DeserializeObject<T>(msg.MessageText);
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
                                _logger.LogWarning($"Unable to deserialize the message. Message is being archived. MessageId: {msg.MessageId}");
                                await _deadLetterQueueClient.SendMessageAsync(msg.MessageText);
                                await _queueClient.DeleteMessageAsync(msg.MessageId, msg.PopReceipt, token);
                                return;
                            }


                            ProcessResult result = await factory.ProcessRequestAsync(request, token);

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


                    }
                }

                _logger.LogInformation($"No additional messages found. Queue: {_queueClient.Name}.");
            }

            catch (Exception ex) //todo: filter for specific exceptions
            {
                _logger.LogError($"An error occured while monitoring queue: {_queueClient.Name}. Details: {ex.Message}");
            }
        }

        private async Task EnsureQueueAsync()
        {
            try
            {
                await _queueClient.CreateIfNotExistsAsync();
            }
            catch (Azure.RequestFailedException ex)
            {
                _logger.LogWarning($"Unable to verify existence of the queue: {_queueClient.Name}. Ensure the queue is created and appropriate permissions have been set. Details: {ex.Message}");


                if (ex.Status == 403 && _config.MonitoredQueueConnectionString.KeyType == Common.DataAccess.ConnectionStrings.StorageAccountConnectionString.AccountKeyType.None)
                {
                    _logger.LogWarning("Please ensure that the queue access policy is set to 'Azure AD User Account'");
                }
            }
        }

        private async Task<bool> PendingMessagesExistAsync(CancellationToken token)
        {
            try
            {
                return (await _queueClient.PeekMessagesAsync(1, token)).Value.Any();
            }
            catch (Azure.RequestFailedException ex)
            {
                _logger.LogWarning($"Unable to peek messages in queue: {_queueClient.Name}. Ensure the queue is created and appropriate permissions have been set. Details: {ex.Message}");

                if (ex.Status == 403 && _config.MonitoredQueueConnectionString.KeyType == Common.DataAccess.ConnectionStrings.StorageAccountConnectionString.AccountKeyType.None)
                {
                    _logger.LogWarning("Please ensure that the queue access policy is set to 'Azure AD User Account'");
                }


                return false;
            }
        }
    }
}
