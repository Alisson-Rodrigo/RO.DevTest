using MediatR;
using RO.DevTest.Domain.Enums;


namespace RO.DevTest.Application.Features.Pages
{
    public class PagedRequest : IRequest<PagedResult>
    {
        public int Page { get; set; } = 1; 
        public int PageSize { get; set; } = 10; 
        public string? OrderBy { get; set; } 
        public bool Ascending { get; set; } = true;
        public string? Search { get; set; } 
    }
}
