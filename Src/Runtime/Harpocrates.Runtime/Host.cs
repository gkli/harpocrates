﻿
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Harpocrates.Runtime
{
    public class Host
    {
        private readonly ILogger _logger;
        private readonly Common.Configuration.IConfigurationManager _config;

        private QueueMonitor<Common.Contracts.RawProcessRequest> _rawMessageQueueMonitor;
        private QueueMonitor<Common.Contracts.FormattedProcessRequest> _formattedMessageQueueMonitor;

        public Host(Common.Configuration.IConfigurationManager config, ILogger<Host> logger)
        {
            _logger = logger;
            _config = config;
        }

        public async Task StartAsync(CancellationToken token)
        {
            _logger.LogInformation($"Monitoring starting: {DateTime.Now}");

            _rawMessageQueueMonitor = new QueueMonitor<Common.Contracts.RawProcessRequest>(_config.RawMessagesQueueName, TimeSpan.FromSeconds(30), _config, _logger);
            _formattedMessageQueueMonitor = new QueueMonitor<Common.Contracts.FormattedProcessRequest>(_config.FormattedMessagesQueueName, TimeSpan.FromSeconds(30), _config, _logger);

            while (!token.IsCancellationRequested)
            {
                List<Task> pendingTasks = new List<Task>();

                // do not await, this can run in the background
              //  pendingTasks.Add(_rawMessageQueueMonitor.ProcessPendingMessagesAsync(token));

                await _formattedMessageQueueMonitor.ProcessPendingMessagesAsync(token);

                //if we're done, but pending tasks are still going, wait for that 
                Task.WaitAll(pendingTasks.ToArray(), token);

                await Task.Delay(2500);
            }

            _logger.LogInformation($"Monitoring stopping: {DateTime.Now}");
        }
    }
}
