using MediatR;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;
using RO.DevTest.Application.Contracts.Infrastructure;
using RO.DevTest.Application.Services.TokenJwt;
using RO.DevTest.Domain.Exception;


namespace RO.DevTest.Application.Features.Auth.Commands.LoginCommand;


public class LoginCommandHandler(IIdentityAbstractor identityAbstractor, IConfiguration configuration, IDistributedCache distributedCache) : IRequestHandler<LoginCommand, LoginResponse>
{
    private readonly IIdentityAbstractor _identityAbstractor = identityAbstractor;
    private readonly IConfiguration _configuration = configuration;
    private readonly IDistributedCache _cache = distributedCache; // Adicione cache distribuído


    public async Task<LoginResponse> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var user = await _identityAbstractor.FindByNameAsync(request.Username);
        if (user == null)
            throw new BadRequestException("Usuário ou senha inválidos");

        var result = await _identityAbstractor.CheckPasswordSignInAsync(user, request.Password);
        if (!result.Succeeded)
            throw new BadRequestException("Usuário ou senha inválidos");

        var roles = await _identityAbstractor.GetRolesAsync(user);
        var serviceToken = new TokenService(_configuration, _identityAbstractor, _cache);
        var token = serviceToken.GenerateJwtToken(user, roles);

        return new LoginResponse
        {
            AccessToken = token,
            Roles = roles,
            IssuedAt = DateTime.UtcNow,
            ExpirationDate = DateTime.UtcNow.AddHours(2)
        };
    }

 
}



