using Confluent.Kafka;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Post.Query.Consumer.Config
{
    public class KafkaConfig
    {
        public string? Topic { get; set; }
        public ConsumerConfig? ConsumerConfig { get; set; }
    }
}
