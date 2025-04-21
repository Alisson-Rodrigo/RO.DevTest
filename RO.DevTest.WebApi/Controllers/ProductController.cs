using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;

namespace RO.DevTest.WebApi.Controllers
{
    [Route("api/[controller]")]
    [OpenApiTags("Products")]
    [ApiController]
    public class ProductController(IMediator mediator) : ControllerBase
    {
        private readonly IMediator _mediator = mediator;

        /*
        [HttpPost]
        [ProducesResponseType(typeof(CreateProductResult), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(CreateProductResult), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreatedProduct {  get; set; }
        */
    }
}
