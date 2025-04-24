using MediatR;
using RO.DevTest.Application.Contracts.Persistance.Repositories;

namespace RO.DevTest.Application.Features.Pages.Queries
{
    public class GetPagedProductsQueryHandler : IRequestHandler<PagedRequest, PagedResult>
    {
        private readonly IProductRepository _productRepository;

        public GetPagedProductsQueryHandler(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        public async Task<PagedResult> Handle(PagedRequest request, CancellationToken cancellationToken)
        {
            var items = await _productRepository.GetPagedAsync(
                request.Page,
                request.PageSize,
                request.OrderBy,
                request.Ascending,
                request.Search
            );

            var totalItems = await _productRepository.GetTotalCountAsync(request.Search);

            return new PagedResult
            {
                Page = request.Page,
                PageSize = request.PageSize,
                TotalItems = totalItems,
                Items = items
            };
        }
    }

}
