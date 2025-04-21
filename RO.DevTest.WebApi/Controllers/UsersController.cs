using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using RO.DevTest.Application.Features.User.Commands.CreateUserCommand;
using RO.DevTest.Application.Features.User.Commands.GetUserCommand;
using RO.DevTest.Application.Features.User.Commands.UpdateUserCommand;

namespace RO.DevTest.WebApi.Controllers;

[Route("api/user")]
[OpenApiTags("Users")]
public class UsersController(IMediator mediator) : Controller {
    private readonly IMediator _mediator = mediator;

    [HttpPost]
    [ProducesResponseType(typeof(CreateUserResult), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(CreateUserResult), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateUser(CreateUserCommand request) {
        CreateUserResult response = await _mediator.Send(request);
        return Created(HttpContext.Request.GetDisplayUrl(), response);
    }

    [Authorize]
    [HttpPut]
    [ProducesResponseType(typeof(UpdateUserResult), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(UpdateUserResult), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UpdateUser(UpdateUserCommand request)
    {
        UpdateUserResult response = await _mediator.Send(request);
        return Ok(response);
    }

    [Authorize]
    [HttpGet]
    [ProducesResponseType(typeof(GetUserResult), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(GetUserResult), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetUser()
    {
        var request = new GetUserCommand();
        GetUserResult response = await _mediator.Send(request);
        return Ok(response);
    }

}
