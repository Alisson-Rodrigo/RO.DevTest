using MediatR;
using Microsoft.AspNetCore.Identity;
using RO.DevTest.Application.Contracts.Infrastructure;
using RO.DevTest.Application.Features.User.Commands.CreateUserCommand;
using RO.DevTest.Application.Services.LoggedUser;
using RO.DevTest.Domain.Exception;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace RO.DevTest.Application.Features.User.Commands.UpdateUserCommand
{
    public class UpdateUserCommandHandler(IIdentityAbstractor identityAbstractor, ILogged logged) : IRequestHandler<UpdateUserCommand, UpdateUserResult>
    {
        private readonly IIdentityAbstractor _identityAbstractor = identityAbstractor;
        private readonly ILogged _logged = logged;

        public async Task<UpdateUserResult> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
        {
            var user = await ValidateRequest(request, cancellationToken);

            user.UserName = request.UserName;
            user.Name = request.Name;
            user.Email = request.Email;

            IdentityResult userCreationResult = await _identityAbstractor.UpdateUserAsync(user);
            if (!userCreationResult.Succeeded)
            {
                throw new BadRequestException(userCreationResult);
            }

            return new UpdateUserResult(user);
        }

        private async Task<Domain.Entities.User> ValidateRequest(UpdateUserCommand request, CancellationToken token)
        {
            var validator = new UpdateUserCommandValidator();
            var user = await _logged.UserLogged();

            var validationResult = await validator.ValidateAsync(request, token);
            if (!validationResult.IsValid)
            {
                throw new BadRequestException(validationResult);
            }

            if (!string.Equals(request.UserName, user.UserName, StringComparison.OrdinalIgnoreCase))
            {
                var existingUser = await _identityAbstractor.FindByNameAsync(request.UserName);
                if (existingUser != null && existingUser.Id != user.Id)
                {
                    throw new BadRequestException("Nome de usuário já está em uso por outra conta");
                }
            }

            if (!string.Equals(request.Email, user.Email, StringComparison.OrdinalIgnoreCase))
            {
                var existingEmail = await _identityAbstractor.FindUserByEmailAsync(request.Email);
                if (existingEmail != null && existingEmail.Id != user.Id)
                {
                    throw new BadRequestException("E-mail já está em uso por outra conta");
                }
            }

            return user;
        }

    }
}

