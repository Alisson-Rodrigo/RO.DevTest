
namespace RO.DevTest.Application.Features.Sale.Queries
{
    public class SalesAnalysisResult
    {
        public int TotalSales { get; set; }
        public float TotalRevenue { get; set; }
        public List<ProductRevenueResult> ProductRevenues { get; set; } = new();
    }

    public class ProductRevenueResult
    {
        public Guid ProductId { get; set; }
        public string ProductName { get; set; }
        public int TotalSold { get; set; }
        public float TotalRevenue { get; set; }
    }

}
