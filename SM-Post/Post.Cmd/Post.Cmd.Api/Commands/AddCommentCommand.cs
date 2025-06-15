using CQRS.Core.Commands;

namespace Post.Cmd.Api.Commands
{
    public class AddCommentCommand : BaseCommand
    {
        public string Comment { get; set; } = null!;
        public string Username { get; set; } = null!;
    }
}
