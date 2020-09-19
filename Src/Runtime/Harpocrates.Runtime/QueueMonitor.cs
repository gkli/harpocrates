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

        public QueueMonitor(string queueName, TimeSpan messageVisibilityTimeout, Common.Configuration.IConfigurationManager config, ILogger logger)
        {
            _logger = logger;
            _config = config;

            _queueClient = Helpers.QueueClientHelper.CreateQueueClient(config, queueName);

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
                        MessageHandler<T> handler = new MessageHandler<T>(_queueClient, _deadLetterQueueClient, _config, _logger);

                        foreach (var msg in messages)
                        {
                            await handler.ProcessMessageAsync(msg, token);
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
