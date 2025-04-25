﻿using MediatR;
using Microsoft.Extensions.Configuration;
using RO.DevTest.Application.Contracts.Infrastructure;
using RO.DevTest.Domain.Exception;

using RO.DevTest.Application.Contracts.Application.Service;

namespace RO.DevTest.Application.Features.Auth.Commands.UpdatePasswordCommand
{
    public class UpdatePasswordHandler : IRequestHandler<UpdatePasswordCommand, Unit>
    {
        private readonly IIdentityAbstractor _identityAbstractor;
        private readonly IConfiguration _configuration;
        private readonly ITokenService _tokenService;
        private readonly ISend _send;


        public UpdatePasswordHandler(IIdentityAbstractor identityAbstractor, IConfiguration configuration, ITokenService tokenService, ISend send)
        {
            _identityAbstractor = identityAbstractor;
            _configuration = configuration;
            _tokenService = tokenService;
            _send = send;
        }

        public async Task<Unit> Handle(UpdatePasswordCommand command, CancellationToken cancellationToken)
        {
            Domain.Entities.User user = await ValidateEmail(command, cancellationToken);
            bool responseEmail =  await SendLinkEmail(command, user);
            if (responseEmail == false)
            {
                throw new BadRequestException("Erro ao enviar o email");
            }

            return Unit.Value; 
        }

        private async Task<Domain.Entities.User> ValidateEmail(UpdatePasswordCommand command, CancellationToken cancellationToken)
        {
            // Validação do comando
            var validator = new UpdatePasswordValidation();
            var response = await validator.ValidateAsync(command, cancellationToken);

            if (!response.IsValid)
            {
                throw new BadRequestException(response);
            }

            // Verifica se o usuário existe
            var user = await _identityAbstractor.FindUserByEmailAsync(command.Email);
            if (user == null)
            {
                throw new BadRequestException("User not found");
            }

            return user;
        }

        private async Task<bool> SendLinkEmail(UpdatePasswordCommand command, Domain.Entities.User user)
        {

            var roles = await _identityAbstractor.GetRolesAsync(user);

            var token = await _tokenService.GeneratePasswordResetToken(user);

            var baseUrl = _configuration.GetValue<string>("AppSettings:PasswordResetUrl");
            var recoveryLink = $"{baseUrl}?token={token}";

            bool responseEmail = _send.SendRecoveryEmail(command.Email.Trim(), recoveryLink);

            return responseEmail;
        }
    }
}