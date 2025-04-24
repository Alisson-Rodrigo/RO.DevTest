using Microsoft.Extensions.DependencyInjection;
using RO.DevTest.Application.Contracts.Application.Service;
using RO.DevTest.Application.Services.LoggedUser;
using RO.DevTest.Application.Services.SMTPEmail;
using RO.DevTest.Application.Services.TokenJwt;

namespace RO.DevTest.Application.IoC
{
    public static class ApplicationDependencyInjector
    {
        public static IServiceCollection InjectApplicationDependencies(this IServiceCollection services)
        {
            services.AddScoped<ILogged, Logged>();
            services.AddScoped<GetTokenRequest>();
            services.AddScoped<ITokenService, TokenService>();
            services.AddScoped<ISend, Send>();


            return services;
        }
    }
}
