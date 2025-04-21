using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;
using RO.DevTest.Application.Contracts.Infrastructure;
using RO.DevTest.Application.Services.TokenJwt;
using RO.DevTest.Domain.Exception;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace RO.DevTest.Application.Features.Auth.Commands.ResetPassword
{
    public class ResetPasswordCommandHandler : IRequestHandler<ResetPasswordCommand, Unit>
    {
        private readonly IIdentityAbstractor _identityAbstractor;
        private readonly IConfiguration _configuration;
        private readonly IDistributedCache _cache; 


        public ResetPasswordCommandHandler(
            IIdentityAbstractor identityAbstractor, IConfiguration configuration, IDistributedCache distributedCache)
        {
            _identityAbstractor = identityAbstractor;
            _configuration = configuration;
            _cache = distributedCache;
        }

        public async Task<Unit> Handle(ResetPasswordCommand command, CancellationToken cancellationToken)
        {
            // Validação dos dados de entrada
            var validator = new ResetPasswordCommandValidator();
            var serviceToken = new TokenService(_configuration, _identityAbstractor, _cache);

            var validationResult = await validator.ValidateAsync(command, cancellationToken);

            if (!validationResult.IsValid)
            {
                throw new BadRequestException(validationResult);
            }

            // Valida o token de recuperação
            if (!serviceToken.ValidatePasswordResetToken(command.Token, out ClaimsPrincipal principal))
            {
                throw new BadRequestException("Token inválido ou expirado");
            }

            // Extrai o email do usuário do token
            var userEmail = principal.FindFirst(ClaimTypes.Email)?.Value;
            if (string.IsNullOrWhiteSpace(userEmail))
            {
                throw new BadRequestException("Token não contém um email válido");
            }

            // Busca o usuário pelo email
            var user = await _identityAbstractor.FindUserByEmailAsync(userEmail);
            if (user == null)
            {
                throw new BadRequestException("Usuário não encontrado");
            }

            var identityToken = await serviceToken.GetIdentityResetToken(userEmail);
            if (string.IsNullOrEmpty(identityToken))
            {
                throw new BadRequestException("Token inválido ou já utilizado.");
            }

            // Reseta a senha do usuário
            var result = await _identityAbstractor.ResetPasswordAsync(user, identityToken, command.Password);

            if (!result.Succeeded)
            {
                throw new BadRequestException(string.Join(", ", result.Errors.Select(e => e.Description)));
            }

            return Unit.Value;
        }
    }
}