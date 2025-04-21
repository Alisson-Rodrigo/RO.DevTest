using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RO.DevTest.Application.Features.User.Commands.UpdateUserCommand
{
    public class UpdateUserResult
    {
        public string Id { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public UpdateUserResult(Domain.Entities.User user)
        {
            Id = user.Id;
            UserName = user.UserName!;
            Email = user.Email!;
            Name = user.Name!;
        }
    }
}
