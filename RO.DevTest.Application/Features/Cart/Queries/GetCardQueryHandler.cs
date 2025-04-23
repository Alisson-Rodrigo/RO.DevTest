using MediatR;
using RO.DevTest.Application.Contracts.Persistance.Repositories;
using RO.DevTest.Application.Services.LoggedUser;
using RO.DevTest.Domain.Exception;

namespace RO.DevTest.Application.Features.Cart.Queries
{

    public class GetCartQueryHandler : IRequestHandler<GetCartQuery, List<CartItemResult>>
    {
        private readonly ICartRepository _cartRepository;
        private readonly IProductRepository _productRepository;
        private readonly ILogged _logged;

        public GetCartQueryHandler(ICartRepository cartRepository, IProductRepository productRepository, ILogged logged)
        {
            _cartRepository = cartRepository;
            _productRepository = productRepository;
            _logged = logged;
        }

        public async Task<List<CartItemResult>> Handle(GetCartQuery request, CancellationToken cancellationToken)
        {
            var user = await _logged.UserLogged();
            if (user == null)
                throw new BadRequestException("Usuário não autenticado");

            var cartItems = await _cartRepository.GetListAsync(user.Id.ToString());

            var result = new List<CartItemResult>();
            foreach (var item in cartItems)
            {
                var product = await _productRepository.GetByIdAsync(item.ProductId);
                if (product != null)
                {
                    result.Add(new CartItemResult
                    {
                        ProductId = item.ProductId,
                        ProductName = product.Name,
                        PrecoUnitario = item.PrecoUnitario,
                        Quantidade = item.Quantidade
                    });
                }
            }

            return result;
        }
    }

}
