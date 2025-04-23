using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using RO.DevTest.Application.Features.Product.Commands.CreatedProductCommand;
using RO.DevTest.Application.Features.Product.Commands.DeleteProductCommand;
using RO.DevTest.Application.Features.Product.Commands.UpdateProductCommand;
using RO.DevTest.Application.Features.Product.Queries.GetAllProductCommand;
using RO.DevTest.Application.Features.Product.Queries.GetProductIdCommand;

namespace RO.DevTest.WebApi.Controllers
{
    [Route("api/product")]
    [OpenApiTags("Products")]
    [ApiController]
    public class ProductController(IMediator mediator) : ControllerBase
    {
        private readonly IMediator _mediator = mediator;

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreatedProduct([FromForm] CreatedProductCommand request)
        {
            await _mediator.Send(request);
            return NoContent();
        }

        [Authorize(Roles = "Admin")]
        [HttpPut]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UpdateProduct([FromForm] UpdateProductCommand request)
        {
            await _mediator.Send(request);
            return NoContent();
        }

        [HttpGet]
        [ProducesResponseType(typeof(List<GetAllProductResult>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]

        public async Task<IActionResult> GetAllProduct ()
        {
            var request = new GetAllProductCommand();
            var products = await _mediator.Send(request);
            return Ok(products);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(GetProductIdResult), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetProductId([FromRoute] Guid id)
        {
            var request = new GetProductIdQuery{ Id = id };
            var response = await _mediator.Send(request);
            return Ok(response);

        }

        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> DeleteProduct([FromRoute] Guid id)
        {
            var request = new DeleteProductCommand { Id = id };
            await _mediator.Send(request);
            return NoContent();

        }

    }
}
