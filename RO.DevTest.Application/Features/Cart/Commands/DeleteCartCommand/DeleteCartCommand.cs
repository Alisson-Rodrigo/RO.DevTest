using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RO.DevTest.Application.Features.Cart.Commands.DeleteCartCommand
{
    public class DeleteCartCommand : IRequest<Unit>
    {
        public Guid Id { get; set; }
    }
}
