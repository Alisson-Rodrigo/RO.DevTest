﻿using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RO.DevTest.Application.Contracts.Persistance.Repositories;
using RO.DevTest.Persistence.Repositories;

namespace RO.DevTest.Persistence.IoC;

public static class PersistenceDependencyInjector {
    /// <summary>
    /// Inject the dependencies of the Persistence layer into an
    /// <see cref="IServiceCollection"/>
    /// </summary>
    /// <param name="services">
    /// The <see cref="IServiceCollection"/> to inject the dependencies into
    /// </param>
    /// <returns>
    /// The <see cref="IServiceCollection"/> with dependencies injected
    /// </returns>
    public static IServiceCollection InjectPersistenceDependencies(
        this IServiceCollection services,
        IConfiguration configuration)  // ← Adicione IConfiguration
    {
        services.AddDbContext<DefaultContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("PostgreSQL")));

        services.AddScoped<IProductRepository, ProductRepository>();
        services.AddScoped<ICartRepository, CartRepository>();
        services.AddScoped<ISaleRepository, SaleRepository>();


        return services;
    }
}
