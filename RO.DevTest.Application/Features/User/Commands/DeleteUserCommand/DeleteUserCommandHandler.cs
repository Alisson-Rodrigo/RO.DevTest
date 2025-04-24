using MediatR;
using RO.DevTest.Application.Contracts.Application.Service;
using RO.DevTest.Application.Contracts.Infrastructure;

namespace RO.DevTest.Application.Features.User.Commands.DeleteUserCommand
{
    public class DeleteUserCommand : IRequest<Unit> { }
    public class DeleteUserCommandHandler : IRequestHandler<DeleteUserCommand, Unit>
    {
        private readonly ILogged _logged;
        private readonly IIdentityAbstractor _identityAbstractor;

        public DeleteUserCommandHandler(ILogged logged, IIdentityAbstractor identityAbstractor) { _logged = logged; _identityAbstractor = identityAbstractor; }

        public async Task<Unit> Handle (DeleteUserCommand command, CancellationToken token)
        {
            var user = await _logged.UserLogged();
            if (user == null)
            {
                throw new UnauthorizedAccessException("Usuário não encontrado");
            }

            await _identityAbstractor.DeleteUser(user);

            return Unit.Value;
        }
    }
}
