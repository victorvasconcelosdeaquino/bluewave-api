using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Builder;            
using System.Reflection;     
using Bluewave.Core.Behaviors;
using Bluewave.Core.Exceptions;
using FluentValidation;
using MediatR;

namespace Bluewave.Core.Extensions;

public static class DependencyInjectionExtensions
{
    public static IServiceCollection AddBluewaveCore(this IServiceCollection services, Assembly applicationAssembly)
    {
        services.AddExceptionHandler<GlobalExceptionHandler>();
        services.AddProblemDetails();

        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(applicationAssembly);
            cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
        });

        services.AddValidatorsFromAssembly(applicationAssembly);

        return services;
    }

    public static IApplicationBuilder UseBluewaveCore(this IApplicationBuilder app)
    {
        app.UseExceptionHandler();
        return app;
    }
}