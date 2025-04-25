using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace RO.DevTest.Application.Contracts.Application.Service
{
    public interface ITokenService
    {
        public string GenerateJwtToken(Domain.Entities.User user, IList<string> roles);
        public Task<string> GetIdentityResetToken(string email);

        public Task<string> GeneratePasswordResetToken(Domain.Entities.User user);

        public bool ValidatePasswordResetToken(string token, out ClaimsPrincipal principal);


    }
}
