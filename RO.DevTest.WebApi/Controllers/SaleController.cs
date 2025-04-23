using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RO.DevTest.Application.Features.Cart.Commands;
using RO.DevTest.Application.Features.Sale.Commands.CreatedSaleCommand;

namespace RO.DevTest.WebApi.Controllers
{
    [Route("api/sale")]
    [ApiController]
    public class SaleController(IMediator mediator) : Controller
    {
        private readonly IMediator _mediator = mediator;

        [Authorize]
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreatedSale([FromForm] CreatedSaleCommand request)
        {
            await _mediator.Send(request);
            return NoContent();
        }
    }
}
