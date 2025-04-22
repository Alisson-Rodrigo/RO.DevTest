using MediatR;

namespace RO.DevTest.Application.Features.Cart.Commands
{
    public class CreatedCartCommand : IRequest<Unit>
    {
        public Guid ProductId { get; set; }
        public int Quantidade { get; set; }
    }
}
