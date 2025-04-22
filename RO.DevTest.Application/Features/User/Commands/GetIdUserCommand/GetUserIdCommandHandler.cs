using MediatR;
using RO.DevTest.Application.Contracts.Infrastructure;
using RO.DevTest.Application.Features.User.Commands.GetUserCommand;
using RO.DevTest.Application.Services.LoggedUser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RO.DevTest.Application.Features.User.Commands.GetIdUserCommand
{
    public class GetUserIdCommandHandler : IRequestHandler<GetUserIdCommand, GetUserIdResult>
    {
        private readonly IIdentityAbstractor _identityAbstractor;
        public GetUserIdCommandHandler(
            IIdentityAbstractor identityAbstractor)
        {
            _identityAbstractor = identityAbstractor;
        }

        public async Task<GetUserIdResult> Handle(GetUserIdCommand command, CancellationToken cancellationToken)
        {
            try
            {
                var user = await _identityAbstractor.FindUserByIdAsync(command.Id.ToString());
                if (user == null)
                {
                    throw new Exception("Usuário não encontrado");
                }

                return new GetUserIdResult(user);
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao buscar usuário");
            }
        }
    }
}
