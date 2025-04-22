using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RO.DevTest.Application.Features.Cart.Commands;
using RO.DevTest.Application.Features.Product.Commands.CreatedProductCommand;

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
    }
}
