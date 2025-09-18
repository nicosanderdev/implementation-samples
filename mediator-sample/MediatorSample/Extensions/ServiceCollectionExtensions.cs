using System.Reflection;
using MediatorSample.Application;
using MediatorSample.Application.Behaviors;
using MediatorSample.Domain.Interfaces;

namespace MediatorSample.Extensions;

public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Scans the specified assemblies for IRequestHandler implementations and registers them with the DI container.
    /// Also registers the ISender and Sender.
    /// </summary>
    /// <param name="services">The IServiceCollection to add the services to.</param>
    /// <param name="assemblies">The assemblies to scan for handlers.</param>
    public static IServiceCollection AddMediator(this IServiceCollection services, params Assembly[] assemblies)
    {
        services.AddScoped<ISender, Sender>();
        
        var handlerTypes = assemblies.SelectMany(a => a.GetTypes())
            .Where(t => t.IsClass && !t.IsAbstract && t.GetInterfaces().Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IRequestHandler<,>)));

        foreach (var handlerType in handlerTypes)
        {
            var serviceTypes = handlerType.GetInterfaces()
                .Where(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IRequestHandler<>));
            
            foreach (var serviceType in serviceTypes)
            {
                services.AddScoped(serviceType, handlerType);
            }
        }
        
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));

        return services;
    }
}