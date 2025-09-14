using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Post.Cmd.Api.Commands;
using Post.Common.Utilities;

namespace Post.Cmd.Api.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class UserGalleryController : ControllerBase
    {
        private readonly ILogger<AddCommentController> _logger;
        private readonly IMediator _mediator;
        private readonly IValidator<UploadImageCommand> _validator;

        public UserGalleryController(ILogger<AddCommentController> logger, IMediator mediator, IValidator<UploadImageCommand> validator)
        {
            _logger = logger;
            _mediator = mediator;
            _validator = validator;
        }

        [Authorize("write")]
        [HttpPut("image")]
        public async Task<IActionResult> AddImage([FromBody] UploadImageCommand command, CancellationToken ct)
        {
            var validationResult = await _validator.ValidateAsync(command);

            if (!validationResult.IsValid)
            {
                validationResult.AddToModelState(ModelState);
                return BadRequest(ModelState);
            }

            await _mediator.Send(command, ct);
            return Ok();
        }
    }
}
