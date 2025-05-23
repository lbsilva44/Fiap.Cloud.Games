using Fiap.Cloud.Games.Application.Interfaces;
using Fiap.Cloud.Games.Application.Services;
using Fiap.Cloud.Games.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Fiap.Cloud.Games.API.Extensions;

public static class DependencyInjectionExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<BaseDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

        services.AddScoped<IUsuarioService, UsuarioService>();
        services.AddScoped<IJwtService, JwtService>();
        services.AddScoped<IJogoService, JogoService>();
        services.AddScoped<IPromocaoService, PromocaoService>();

        return services;
    }
}