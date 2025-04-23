
namespace RO.DevTest.Application.Features.Pages
{
    public class PagedResult
    {
        public int TotalItems { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int TotalPages => (int)Math.Ceiling((double)TotalItems / PageSize);

        public List<Domain.Entities.Product> Items { get; set; } = new List<Domain.Entities.Product>();
    }

}
