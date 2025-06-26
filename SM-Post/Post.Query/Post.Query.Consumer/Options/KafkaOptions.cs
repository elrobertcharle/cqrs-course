using Confluent.Kafka;

namespace Post.Query.Consumer.Options
{
    public class KafkaOptions
    {
        public string? Topic { get; set; }
        public ConsumerConfig? ConsumerConfig { get; set; }
    }
}
