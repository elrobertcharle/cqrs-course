using FluentValidation;
using Post.Cmd.Api.Commands;

namespace Post.Cmd.Api.Validators
{
    public class UploadImageCommandValidator : AbstractValidator<UploadImageCommand>
    {
        public UploadImageCommandValidator()
        {
            RuleFor(x => x.Title).NotEmpty();
            RuleFor(x => x.Image).NotNull();
        }
    }
}
