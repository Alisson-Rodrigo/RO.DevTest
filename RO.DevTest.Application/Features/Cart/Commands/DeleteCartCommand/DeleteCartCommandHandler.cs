using MediatR;
using RO.DevTest.Application.Contracts.Persistance.Repositories;
using RO.DevTest.Application.Services.LoggedUser;
using RO.DevTest.Domain.Exception;


namespace RO.DevTest.Application.Features.Cart.Commands.DeleteCartCommand
{
    public class DeleteCartCommandHandler : IRequestHandler<DeleteCartCommand, Unit>
    {
        private readonly ILogged _logged;
        private readonly ICartRepository _cartRepository;

        public DeleteCartCommandHandler(ICartRepository cartRepository, ILogged logged)
        {
            _logged = logged;
            _cartRepository = cartRepository;
        }

        public async Task<Unit> Handle(DeleteCartCommand command, CancellationToken cancellationToken)
        {
            var user = await _logged.UserLogged();

            if (user == null) throw new BadRequestException("Erro ao recuperar usuário");

            var cartExists = await _cartRepository.GetItemAsync(user.Id, command.Id);

            if (cartExists == null) throw new BadRequestException("Produto no carrinho não encontrado");

            _cartRepository.Delete(cartExists);

            return Unit.Value;
        }
    }
}
