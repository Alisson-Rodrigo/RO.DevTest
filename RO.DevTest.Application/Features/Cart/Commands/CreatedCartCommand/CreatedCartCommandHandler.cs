using MediatR;
using Microsoft.AspNetCore.Http;
using RO.DevTest.Application.Contracts.Persistance.Repositories;
using RO.DevTest.Application.Services.LoggedUser;
using RO.DevTest.Domain.Entities;

namespace RO.DevTest.Application.Features.Cart.Commands
{
    public class CreatedCartCommandHandler : IRequestHandler<CreatedCartCommand, Unit>
    {
        private readonly ICartRepository _cartRepository;
        private readonly IProductRepository _productRepository;
        private readonly ILogged _logged;

        public CreatedCartCommandHandler(
            ICartRepository cartRepository,
            IProductRepository productRepository,
            ILogged logged)
        {
            _cartRepository = cartRepository;
            _productRepository = productRepository;
            _logged = logged;
        }

        public async Task<Unit> Handle(CreatedCartCommand request, CancellationToken cancellationToken)
        {
            var user = await _logged.UserLogged();

            var product = _productRepository.Get(p => p.Id == request.ProductId);
            if (product == null)
                throw new BadHttpRequestException("Produto não encontrado");

            var itemExistente = await _cartRepository.GetItemAsync(user.Id, request.ProductId);

            if (itemExistente != null)
            {
                itemExistente.Quantidade += request.Quantidade;
                _cartRepository.Update(itemExistente);
            }
            else
            {
                var novoItem = new CartItem
                {
                    UserId = user.Id,
                    ProductId = request.ProductId,
                    Quantidade = request.Quantidade,
                    PrecoUnitario = product.Price
                };

                await _cartRepository.CreateAsync(novoItem, cancellationToken);
            }

            return Unit.Value;
        }
    }
}
