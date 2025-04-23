using MediatR;
using RO.DevTest.Application.Contracts.Persistance.Repositories;
using RO.DevTest.Application.Features.Product.Commands.GetAllProductCommand;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RO.DevTest.Application.Features.Product.Queries.GetProductIdCommand
{
    public class GetProductIdQueryHandler : IRequestHandler<GetProductIdQuery, GetProductIdResult>
    {
        private readonly IProductRepository _productRepository;
        public GetProductIdQueryHandler(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        public async Task<GetProductIdResult> Handle(GetProductIdQuery command, CancellationToken cancellationToken)
        {
            var product = _productRepository.Get(x => x.Id == command.Id);

            if (product == null)
            {
                throw new ArgumentNullException(nameof(product));
            }

            return new GetProductIdResult(product);
        }
    }
}
