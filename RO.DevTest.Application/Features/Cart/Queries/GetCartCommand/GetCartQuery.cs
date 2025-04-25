using MediatR;

namespace RO.DevTest.Application.Features.Cart.Queries.GetCartCommand
{
    public class GetCartQuery : IRequest<List<CartItemResult>>
    {
    }

}
