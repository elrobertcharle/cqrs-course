using Confluent.Kafka;

namespace Post.Cmd.Api.Producer.Options
{
    public class KafkaOptions
    {
        public string IncomingTopic { get; set; } = null!;
        public string OutgoingTopic { get; set; } = null!;
        public ConsumerConfig ConsumerConfig { get; set; } = null!;
        public ProducerConfig ProducerConfig { get; set; } = null!;
    }
}
