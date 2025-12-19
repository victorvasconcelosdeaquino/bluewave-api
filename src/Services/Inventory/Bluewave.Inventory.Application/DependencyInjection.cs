using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Bluewave.Inventory.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        // Registra o MediatR para ler os Handlers deste projeto
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));

        // Aqui você também registraria o AutoMapper e Validadores FluentValidation se tivesse

        return services;
    }
}