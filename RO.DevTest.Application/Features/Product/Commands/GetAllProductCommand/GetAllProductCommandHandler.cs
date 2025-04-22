using MediatR;
using Microsoft.AspNetCore.Http;
using RO.DevTest.Application.Contracts.Persistance.Repositories;

namespace RO.DevTest.Application.Features.Product.Commands.GetAllProductCommand
{
    public class GetAllProductCommand : IRequest<List<GetAllProductResult>> { } // Mudança no retorno

    public class GetAllProductCommandHandler : IRequestHandler<GetAllProductCommand, List<GetAllProductResult>>
    {
        private readonly IProductRepository _productRepository;

        public GetAllProductCommandHandler(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        public async Task<List<GetAllProductResult>> Handle(GetAllProductCommand request, CancellationToken cancellationToken)
        {
            var products = await _productRepository.GetAllActiveProducts();

            if (!products.Any())
            {
                throw new BadHttpRequestException("Não há produtos cadastrados.");
            }

            return products.Select(product => new GetAllProductResult(product)).ToList();
        }
    }

}
