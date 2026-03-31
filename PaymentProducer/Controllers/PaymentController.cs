using Confluent.Kafka;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using PaymentProducer.Models;
using System.Text.Json;

namespace PaymentProducer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentController : ControllerBase
    {

        // Inject logger and Kafka configuration using IOptions
        private readonly KafkaIoption _kafkaIoption;
        private readonly ILogger<PaymentController> _logger;
        public PaymentController(IOptions<KafkaIoption> kafkaIoption, ILogger<PaymentController> logger)
        {
            _kafkaIoption = kafkaIoption.Value;
            _logger = logger;

        }

        [HttpPost]
        public async Task<IActionResult> Payment(Payment payment)
        {
            //  Validate incoming request body
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid payment request");
                return BadRequest(ModelState);
            }

            _logger.LogInformation("creating payment {payment}", payment);

            //  If payment failed, stop processing
            if (!payment.IsSuccess)
            {
                _logger.LogWarning("Payment not successful: {@payment}", payment);
                return BadRequest(payment);
            }

            try
            {
                //  Create Kafka message 
                var message = new Message<Null, string>()
                {
                    Value = JsonSerializer.Serialize(payment)
                };

                //Configure Kafka producer
                var producerConfig = new ProducerConfig()
                {
                    BootstrapServers = _kafkaIoption.HostName,
                    Acks = Acks.All
                };

                //Send message to Kafka topic
                using var producer = new ProducerBuilder<Null, string>(producerConfig).Build();
                var result = await producer.ProduceAsync(_kafkaIoption.TopicName, message);

                _logger.LogInformation("Message delivered to Topic: {topic}, Partition: {partition}, Offset: {offset}", result.Topic, result.Partition, result.Offset);

                producer.Dispose();

                return Ok(payment);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error producing Kafka message");
                return BadRequest(ex);
            }
        }

    }
}
