using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using RO.DevTest.Application.Contracts.Infrastructure;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace RO.DevTest.Application.Features.Auth.Commands.LoginCommand;


public class LoginCommandHandler(IIdentityAbstractor identityAbstractor, IConfiguration configuration) : IRequestHandler<LoginCommand, LoginResponse>
{
    private readonly IIdentityAbstractor _identityAbstractor = identityAbstractor;
    private readonly IConfiguration _configuration = configuration;

    public async Task<LoginResponse> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var user = await _identityAbstractor.FindByNameAsync(request.Username);
        if (user == null)
            throw new UnauthorizedAccessException("Usuário ou senha inválidos");

        var result = await _identityAbstractor.CheckPasswordSignInAsync(user, request.Password);
        if (!result.Succeeded)
            throw new UnauthorizedAccessException("Usuário ou senha inválidos");

        var roles = await _identityAbstractor.GetRolesAsync(user);
        var token = GenerateJwtToken(user, roles);

        return new LoginResponse
        {
            AccessToken = token,
            Roles = roles,
            IssuedAt = DateTime.UtcNow,
            ExpirationDate = DateTime.UtcNow.AddHours(2)
        };
    }

    private string GenerateJwtToken(Domain.Entities.User user, IList<string> roles)
    {
        var jwtSettings = _configuration.GetSection("JwtSettings");
        var key = Encoding.UTF8.GetBytes(jwtSettings["Key"]!);
        var issuer = jwtSettings["Issuer"];
        var audience = jwtSettings["Audience"];

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id),
            new(ClaimTypes.Name, user.UserName!),
            new(ClaimTypes.Email, user.Email!)
        };

        foreach (var role in roles)
            claims.Add(new Claim(ClaimTypes.Role, role));

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddHours(2),
            Issuer = issuer,
            Audience = audience,
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(key),
                SecurityAlgorithms.HmacSha256Signature)
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }
}



