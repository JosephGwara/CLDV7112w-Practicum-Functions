using System;
using Azure.Messaging.EventHubs;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace healthdatastreams
{
    public class EvenHubDataInjester
    {
        private readonly ILogger<EvenHubDataInjester> _logger;

        public EvenHubDataInjester(ILogger<EvenHubDataInjester> logger)
        {
            _logger = logger;
        }

        [Function(nameof(EvenHubDataInjester))]
        public void Run([EventHubTrigger("realtimedatastreams", Connection = "healthdatastreams_RootManageSharedAccessKey_EVENTHUB")] EventData[] events)
        {
            foreach (EventData @event in events)
            {
                _logger.LogInformation("Event Body: {body}", @event.Body);
                _logger.LogInformation("Event Content-Type: {contentType}", @event.ContentType);
            }
        }
    }
}
