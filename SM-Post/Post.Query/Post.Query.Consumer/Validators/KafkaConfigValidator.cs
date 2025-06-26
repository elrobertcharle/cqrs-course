using FluentValidation;
using Post.Query.Consumer.Options;

namespace Post.Query.Consumer.Validators
{
    public class KafkaOptionsValidator : AbstractValidator<KafkaOptions>
    {
        public KafkaOptionsValidator()
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
