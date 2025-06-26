using FluentValidation;
using Post.Cmd.Api.Options;

namespace Post.Cmd.Api.Validators
{
    public class KafkaProducerOptionsValidator : AbstractValidator<KafkaProducerOptions>
    {
        public KafkaProducerOptionsValidator()
        {
            RuleFor(x => x.Topic).NotEmpty();
        }
    }
}
