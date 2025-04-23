using MediatR;

namespace RO.DevTest.Application.Features.Cart.Queries
{
    public class GetCartQuery : IRequest<List<CartItemResult>>
    {
    }

}
