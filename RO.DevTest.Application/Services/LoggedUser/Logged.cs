using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using RO.DevTest.Application.Contracts.Infrastructure;
using RO.DevTest.Application.Services.TokenJwt;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace RO.DevTest.Application.Services.LoggedUser
{
    public class Logged : ILogged
    {
        private readonly IIdentityAbstractor _identityAbstractor;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<Logged> _logger;

        public Logged(
            IIdentityAbstractor identityAbstractor,
            IHttpContextAccessor httpContextAccessor,
            ILogger<Logged> logger)
        {
            _identityAbstractor = identityAbstractor;
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
        }

        public async Task<Domain.Entities.User> UserLogged()
        {
            try
            {
                var userId = _httpContextAccessor.HttpContext?.User?
                    .FindFirstValue(ClaimTypes.NameIdentifier);

                if (string.IsNullOrEmpty(userId))
                {
                    _logger.LogWarning("Claim NameIdentifier não encontrado no token");
                    throw new UnauthorizedAccessException("Usuário não autenticado");
                }

                var user = await _identityAbstractor.FindUserByIdAsync(userId);

                if (user == null)
                {
                    _logger.LogError("Usuário com ID {UserId} não encontrado", userId);
                    throw new KeyNotFoundException("Usuário não encontrado");
                }

                return user;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao obter usuário logado");
                throw;
            }
        }

        public async Task<bool> IsInRole(string role)
        {
            var user = await UserLogged();
            var roles = await _identityAbstractor.GetRolesAsync(user);
            Console.WriteLine(roles.FirstOrDefault()?.ToString());
            return roles.Contains(role);
        }

    }
}




