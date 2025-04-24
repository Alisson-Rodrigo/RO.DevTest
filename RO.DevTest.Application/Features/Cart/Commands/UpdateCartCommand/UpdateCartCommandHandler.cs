using MediatR;
using RO.DevTest.Application.Contracts.Application.Service;
using RO.DevTest.Application.Contracts.Persistance.Repositories;
using RO.DevTest.Domain.Exception;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace RO.DevTest.Application.Features.Cart.Commands.UpdateCartCommand
{
    public class UpdateCartCommandHandler : IRequestHandler<UpdateCartCommand, Unit>
    {
        private readonly ILogged _logged;
        private readonly ICartRepository _cartRepository;

        public UpdateCartCommandHandler(ILogged logged, ICartRepository cartRepository)
        {
            _logged = logged;
            _cartRepository = cartRepository;
        }

        public async Task<Unit> Handle(UpdateCartCommand request, CancellationToken cancellationToken)
        {
            var user = await _logged.UserLogged();
            if (user == null)
                throw new BadRequestException("Usuário não autenticado");

            var cartExists = await _cartRepository.GetItemAsync(user.Id, request.ProductId);
            if (cartExists == null)
                throw new BadRequestException("Produto não encontrado no carrinho");

            if (request.Quantidade <= 0)
            {
                // Se a quantidade for zero ou negativa, remover do carrinho
                _cartRepository.Delete(cartExists);
            }
            else
            {
                cartExists.Quantidade = request.Quantidade;
                _cartRepository.Update(cartExists);
            }

            return Unit.Value;
        }
    }
}
