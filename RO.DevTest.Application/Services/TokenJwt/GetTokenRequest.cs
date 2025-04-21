using Microsoft.AspNetCore.Http;

namespace RO.DevTest.Application.Services.TokenJwt
{
    public class GetTokenRequest
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
    
        public GetTokenRequest(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }
        public string GetToken()
        {
            var authentication = _httpContextAccessor.HttpContext!.Request.Headers.Authorization.ToString();
            return authentication["Bearer ".Length..].Trim();
        }
    }
}
