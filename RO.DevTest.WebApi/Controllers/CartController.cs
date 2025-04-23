using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RO.DevTest.Application.Features.Cart.Commands;
using RO.DevTest.Application.Features.Cart.Commands.DeleteCartCommand;
using RO.DevTest.Application.Features.Cart.Queries.GetCartCommand;

namespace RO.DevTest.WebApi.Controllers
{
    [Route("api/cart")]
    [ApiController]
    public class CartController(IMediator mediator) : Controller
    {
        private readonly IMediator _mediator = mediator;

        [Authorize]
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreatedCart([FromForm] CreatedCartCommand request)
        {
            await _mediator.Send(request);
            return NoContent();
        }

        [Authorize]
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> DeleteCart([FromRoute] Guid id)
        {
            var command = new DeleteCartCommand{ Id = id};
            await _mediator.Send(command);
            return NoContent();
        }

        [Authorize]
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetCart()
        {
            var request = new GetCartQuery();
            var response = await _mediator.Send(request);
            return Ok(response);
        }
    }
}
