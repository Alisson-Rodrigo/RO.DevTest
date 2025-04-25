using MediatR;
using RO.DevTest.Application.Features.User.Commands.CreateUserCommand;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RO.DevTest.Application.Features.User.Commands.UpdateUserCommand
{
    public class UpdateUserCommand : IRequest<UpdateUserResult>
    {
        public string UserName { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public Domain.Entities.User AssignTo()
        {
            return new Domain.Entities.User
            {
                UserName = UserName,
                Email = Email,
                Name = Name,
            };
        }
    }
}
