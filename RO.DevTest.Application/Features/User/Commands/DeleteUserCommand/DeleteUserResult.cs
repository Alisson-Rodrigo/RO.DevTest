using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RO.DevTest.Application.Features.User.Commands.DeleteUserCommand
{
    public class DeleteUserResult
    {
        public string Id { get; set; }

        public DeleteUserResult (string id) {  Id = id; }
    }
}
