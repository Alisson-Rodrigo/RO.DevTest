using MediatR;

namespace RO.DevTest.Application.Features.Product.Queries.GetProductIdCommand
{
    public class GetProductIdQuery : IRequest<GetProductIdResult>
    {
        public Guid Id { get; set; }
    }
}
