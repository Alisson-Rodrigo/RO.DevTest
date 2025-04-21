using Microsoft.Extensions.DependencyInjection;
using RO.DevTest.Application.Services.LoggedUser;
using RO.DevTest.Application.Services.TokenJwt;

namespace RO.DevTest.Application.IoC
{
    public static class ApplicationDependencyInjector
    {
        public static IServiceCollection InjectApplicationDependencies(this IServiceCollection services)
        {
            services.AddScoped<ILogged, Logged>();
            services.AddScoped<GetTokenRequest>();


            return services;
        }
    }
}
