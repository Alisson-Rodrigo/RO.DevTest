using MediatR;
using RO.DevTest.Application.Contracts.Application.Service;
using RO.DevTest.Application.Contracts.Persistance.Repositories;
using RO.DevTest.Domain.Entities;
using RO.DevTest.Domain.Exception;

namespace RO.DevTest.Application.Features.Sale.Commands.CreatedSaleCommand
{
    public class CreateSaleCommandHandler : IRequestHandler<CreatedSaleCommand, Guid>
    {
        private readonly ILogged _logged;
        private readonly ICartRepository _cartRepository;
        private readonly ISaleRepository _saleRepository;
        private readonly IProductRepository _productRepository;

        public CreateSaleCommandHandler(ILogged logged, ICartRepository cartRepository, ISaleRepository saleRepository, IProductRepository productRepository)
        {
            _logged = logged;
            _cartRepository = cartRepository;
            _saleRepository = saleRepository;
            _productRepository = productRepository;
        }

        public async Task<Guid> Handle(CreatedSaleCommand request, CancellationToken cancellationToken)
        {
            var user = await _logged.UserLogged();
            if (user == null) throw new BadRequestException("Usuário não autenticado");

            var cartItems = await _cartRepository.GetListAsync(user.Id.ToString());
            if (cartItems == null || !cartItems.Any())
                throw new BadRequestException("Carrinho vazio");

            // Valida cada item do carrinho
            foreach (var item in cartItems)
            {
                var product = await _productRepository.GetByIdAsync(item.ProductId);
                if (product == null)
                    throw new BadRequestException($"Produto {item.ProductId} não encontrado");

                if (product.Stock <= 0)
                    throw new BadRequestException($"Produto '{product.Name}' está fora de estoque");

                if (item.Quantidade > product.Stock)
                    throw new BadRequestException($"Estoque insuficiente para o produto '{product.Name}'. Quantidade disponível: {product.Stock}, solicitada: {item.Quantidade}");
            }

            var sale = new Domain.Entities.Sale
            {
                UserId = user.Id,
                DataVenda = DateTime.UtcNow,
                Itens = cartItems.Select(item => new SaleItem
                {
                    ProductId = item.ProductId,
                    Quantidade = item.Quantidade,
                    PrecoUnitario = item.PrecoUnitario
                }).ToList()
            };

            sale.Total = sale.Itens.Sum(i => i.PrecoUnitario * i.Quantidade);
            Console.WriteLine($"Total calculado: {sale.Total}");

            await _saleRepository.CreateAsync(sale);

            await _cartRepository.DeleteAllAsync(user.Id);

            foreach (var item in sale.Itens)
            {
                var product = await _productRepository.GetByIdAsync(item.ProductId);
                if (product != null)
                {
                    product.Stock -= item.Quantidade;
                    _productRepository.Update(product);
                }
            }

            return sale.Id;
        }
    }
}
