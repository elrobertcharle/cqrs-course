using FluentValidation;
using Post.Cmd.Api.Producer.Options;

namespace Post.Cmd.Api.Producer.Validators
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

            RuleFor(x => x.IncomingTopic).NotEmpty();
            RuleFor(x => x.OutgoingTopic).NotEmpty();
        }
    }
}
