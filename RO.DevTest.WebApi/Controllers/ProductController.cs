using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using RO.DevTest.Application.Features.Product.Commands.CreatedProductCommand;

namespace RO.DevTest.WebApi.Controllers
{
    [Route("api/[controller]")]
    [OpenApiTags("Products")]
    [ApiController]
    public class ProductController(IMediator mediator) : ControllerBase
    {
        private readonly IMediator _mediator = mediator;

        [Authorize]
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreatedProduct([FromForm] CreatedProductCommand request)
        {
            await _mediator.Send(request);
            return NoContent();
        }
        
    }
}
