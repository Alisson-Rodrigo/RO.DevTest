using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RO.DevTest.Application.Features.User.Queries.GetIdUserCommand
{
    public class GetUserIdResult
    {
        public string UserName { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;

        public GetUserIdResult(Domain.Entities.User user)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));

            UserName = user.UserName ?? string.Empty;
            Email = user.Email ?? string.Empty;
            Name = user.Name ?? string.Empty;
        }
    }
}
