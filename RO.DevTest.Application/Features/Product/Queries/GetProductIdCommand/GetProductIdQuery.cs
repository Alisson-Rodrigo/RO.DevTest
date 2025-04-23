using MediatR;
using RO.DevTest.Application.Features.User.Commands.GetIdUserCommand;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RO.DevTest.Application.Features.Product.Queries.GetProductIdCommand
{
    public class GetProductIdQuery : IRequest<GetProductIdResult>
    {
        public Guid Id { get; set; }
    }
}
