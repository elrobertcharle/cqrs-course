using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;
using Post.Query.Consumer.Config;

namespace Post.Query.Consumer.Validators
{
    public class KafkaConfigValidator : AbstractValidator<KafkaConfig>
    {
        public KafkaConfigValidator()
        {
            RuleFor(x => x.ConsumerConfig).NotNull().DependentRules(() =>
            {
                RuleFor(x => x.ConsumerConfig!.BootstrapServers).NotEmpty();

                RuleFor(x => x.ConsumerConfig!.GroupId).NotEmpty();
            });

            RuleFor(x => x.Topic).NotEmpty();
        }
    }
}
