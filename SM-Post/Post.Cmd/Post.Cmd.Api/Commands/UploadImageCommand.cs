using CQRS.Core.Commands;
using MediatR;

namespace Post.Cmd.Api.Commands
{
    public class UploadImageCommand : BaseCommand, IRequest
    {
        public string Title { get; set; } = null!;
        public IFormFile Image { get; set; } = null!;
    }
}
