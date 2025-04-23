using MediatR;
using RO.DevTest.Application.Contracts.Infrastructure;


namespace RO.DevTest.Application.Features.User.Queries.GetIdUserCommand
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
