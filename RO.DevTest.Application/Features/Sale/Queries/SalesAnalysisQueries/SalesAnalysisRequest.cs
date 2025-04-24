using MediatR;

namespace RO.DevTest.Application.Features.Sale.Queries.SalesAnalysisQueries
{
    public class SalesAnalysisRequest : IRequest<SalesAnalysisResult>
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; } = DateTime.UtcNow;
    }

}
