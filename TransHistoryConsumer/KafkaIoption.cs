namespace TransHistoryConsumer
{
    public class KafkaIoption
    {
        public string HostName { get; set; }
        public string TopicName { get; set; }
        public string ConsumerGroupId { get; set; }
        public string ConsumerClientId { get; set; }
    }
}
