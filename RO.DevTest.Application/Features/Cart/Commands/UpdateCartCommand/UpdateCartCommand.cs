using MediatR;

namespace RO.DevTest.Application.Features.Cart.Commands.UpdateCartCommand
{
    public class UpdateCartCommand : IRequest<Unit>
    {
        public Guid ProductId { get; set; }
        public int Quantidade { get; set; }
    }
}
