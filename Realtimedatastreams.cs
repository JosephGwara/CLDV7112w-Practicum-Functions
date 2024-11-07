using System;
using Azure.Messaging.EventHubs;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using System.Text;
using System.Text.Json;

namespace healthdatastreams
{

    public class Realtimedatastreams
    {
        private readonly ILogger<Realtimedatastreams> _logger;

        public Realtimedatastreams(ILogger<Realtimedatastreams> logger)
        {
            _logger = logger;
        }

        [Function(nameof(Realtimedatastreams))]
        public async Task RunAsync([EventHubTrigger("realtimedatastreams", Connection = "healthdatastreams_RootManageSharedAccessKey_EVENTHUB")] EventData[] events)
        {
            foreach (EventData @event in events)
            {
                _logger.LogInformation("Event Body: {body}", @event.Body);
                _logger.LogInformation("Event Content-Type: {contentType}", @event.ContentType);
                string messageBody = Encoding.UTF8.GetString(@event.EventBody);

                 var healthData =  JsonSerializer.Deserialize<HealthMetrics>(messageBody);

                if (healthData != null && IsCriticalEvent(healthData))
                    {
                        await TriggerAlertAsync(healthData);
                    }


            
            }
        }
         private static bool IsCriticalEvent(HealthMetrics healthMetrics)
        {
            return healthMetrics.MetricValue > 120 || healthMetrics.MetricValue < 50;
        }

        private static Task TriggerAlertAsync(HealthMetrics healthMetrics)
        {
            Console.WriteLine($"ALERT! Critical health event for patient {healthMetrics.PatientId}");
            return Task.CompletedTask;
        }

    }
}
  public class HealthMetrics
    {   
        public  int? MetricId { get; set; }
        public  int? PatientId { get; set; }
        public  int? DeviceId { get; set; }
        public string? MetricType { get; set; }
        public double? MetricValue { get; set; }
        public string? Unit { get; set; }
        public DateTime Timestamp { get; set; }
    } 