using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Bluewave.Inventory.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        // Register the MediatR to read the Handlers from this project
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));

        // TODO: register AutoMapper profiles and FluentValidation validators

        return services;
    }
}