using MediatR;
using RO.DevTest.Application.Contracts.Infrastructure;
using RO.DevTest.Application.Services.LoggedUser;

namespace RO.DevTest.Application.Features.User.Commands.DeleteUserCommand
{
    public class DeleteUserCommand : IRequest<DeleteUserResult> { }
    public class DeleteUserCommandHandler : IRequestHandler<DeleteUserCommand, DeleteUserResult>
    {
        private readonly ILogged _logged;
        private readonly IIdentityAbstractor _identityAbstractor;

        public DeleteUserCommandHandler(ILogged logged, IIdentityAbstractor identityAbstractor) { _logged = logged; _identityAbstractor = identityAbstractor; }

        public async Task<DeleteUserResult> Handle (DeleteUserCommand command, CancellationToken token)
        {
            var user = await _logged.UserLogged();
            if (user == null)
            {
                throw new UnauthorizedAccessException("Usuário não encontrado");
            }

            await _identityAbstractor.DeleteUser(user);

            return new DeleteUserResult(user.Id);
        }
    }
}
