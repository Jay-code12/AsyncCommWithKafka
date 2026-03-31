using Confluent.Kafka;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Text.Json;
using TransactionHistory.Models;
using TransHistoryConsumer.Models;

namespace TransHistoryConsumer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HistoryController : ControllerBase
    {
        // Inject logger and Kafka configuration using IOptions
        private readonly KafkaIoption _kafkaIoption;
        private readonly ILogger<HistoryController> _logger;
        public HistoryController(IOptions<KafkaIoption> kafkaIoption, ILogger<HistoryController> logger)
        {
            _kafkaIoption = kafkaIoption.Value;
            _logger = logger;

        }


        [HttpGet]
        public async Task<IActionResult> PaymentHistory()
        {
            try
            {
                var consumerConfig = new ConsumerConfig()
                {
                    BootstrapServers = _kafkaIoption.HostName,
                    ClientId = _kafkaIoption.ConsumerGroupId,
                    GroupId = _kafkaIoption.ConsumerClientId,
                };


                using (var consumer = new ConsumerBuilder<Null, string>(consumerConfig).Build())
                {
                    consumer.Subscribe(_kafkaIoption.TopicName);
                    var consumerData = consumer.Consume(TimeSpan.FromSeconds(3));
                    if (consumerData is not null)
                    {
                        var paymentHistory = JsonSerializer.Deserialize<HistoryDto>(consumerData.Message.Value);

                        var paymentReport = new History()
                        {
                            Id = Guid.NewGuid(),
                            PaymentId = paymentHistory.Id,
                            IsSuccess = paymentHistory.IsSuccess,
                            Type = paymentHistory.Type,
                            Amount = paymentHistory.Amount
                        };
                        return Ok(paymentReport);
                    }
                };
                return NotFound();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error Consuming Kafka message Topic: {topic} GroupId: {grouupId} ConsumerId: {consumerId}", _kafkaIoption.TopicName, _kafkaIoption.ConsumerGroupId, _kafkaIoption.ConsumerClientId);
                return BadRequest();
            }

        }
    }
}
