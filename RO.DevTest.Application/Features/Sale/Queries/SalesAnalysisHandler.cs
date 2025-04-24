using MediatR;
using RO.DevTest.Application.Contracts.Persistance.Repositories;


namespace RO.DevTest.Application.Features.Sale.Queries
{
    public class SalesAnalysisHandler : IRequestHandler<SalesAnalysisRequest, SalesAnalysisResult>
    {
        private readonly ISaleRepository _saleRepository;

        public SalesAnalysisHandler(ISaleRepository saleRepository)
        {
            _saleRepository = saleRepository;
        }

        public async Task<SalesAnalysisResult> Handle(SalesAnalysisRequest request, CancellationToken cancellationToken)
        {
            request.StartDate = DateTime.SpecifyKind(request.StartDate, DateTimeKind.Utc);
            request.EndDate = DateTime.SpecifyKind(request.EndDate, DateTimeKind.Utc);

            var sales = await _saleRepository.GetSalesByPeriod(request.StartDate, request.EndDate);

            var totalSales = sales.Count;
            var totalRevenue = sales
                .SelectMany(s => s.Itens)
                .Sum(i => i.Quantidade * i.PrecoUnitario);

            var productGroups = sales
                .SelectMany(s => s.Itens)
                .GroupBy(i => i.ProductId)
                .Select(g => new ProductRevenueResult
                {
                    ProductId = g.Key,
                    ProductName = g.First().Product.Name,
                    TotalSold = g.Sum(i => i.Quantidade),
                    TotalRevenue = g.Sum(i => i.Quantidade * i.PrecoUnitario)
                })
                .ToList();

            return new SalesAnalysisResult
            {
                TotalSales = totalSales,
                TotalRevenue = totalRevenue,
                ProductRevenues = productGroups
            };
        }
    }

}
