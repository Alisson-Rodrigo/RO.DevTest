using MediatR;
using RO.DevTest.Application.Features.User.Commands.CreateUserCommand;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RO.DevTest.Application.Features.User.Commands.GetIdUserCommand
{
    public class GetUserIdCommand : IRequest<GetUserIdResult>
    {
        public Guid Id { get; set; }
    }
}
