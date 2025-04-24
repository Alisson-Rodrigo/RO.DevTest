using MediatR;
using RO.DevTest.Application.Contracts.Infrastructure;
using RO.DevTest.Domain.Exception;


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
                    // Aqui você lança a BadRequestException com a mensagem exata que o teste espera
                    throw new BadRequestException("Usuário não encontrado");
                }

                return new GetUserIdResult(user);
            }
            catch (BadRequestException)
            {
                // Se a exceção já for BadRequestException, apenas repassa
                throw;
            }
            catch (Exception)
            {
                // Qualquer outro erro entra aqui com a mensagem genérica
                throw new BadRequestException("Erro ao buscar usuário");
            }
        }

    }
}
