using Microsoft.Extensions.DependencyInjection;
using FluentValidation;
using LoginNet.Application.Common.Interfaces;
using System.Reflection;

namespace LoginNet.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            var assembly = Assembly.GetExecutingAssembly();
            services.AddValidatorsFromAssembly(assembly);

            services.AddMediatorHandlers(assembly);

            return services;
        }

        private static IServiceCollection AddMediatorHandlers(this IServiceCollection services, Assembly assembly)
        {
            var handlerType = typeof(IRequestHandler<,>);
            var behaviorType = typeof(IPipelineBehavior<,>);

            var types = assembly.GetTypes()
                .Where(t => t is { IsClass: true, IsAbstract: false });

            foreach (var type in types)
            {
                // Register Handlers
                var interfaces = type.GetInterfaces()
                    .Where(i => i.IsGenericType && i.GetGenericTypeDefinition() == handlerType);

                foreach (var iface in interfaces)
                {
                    if (type.IsGenericTypeDefinition)
                        services.AddScoped(iface.GetGenericTypeDefinition(), type);
                    else
                        services.AddScoped(iface, type);
                }

                // Register Behaviors
                var behaviorInterfaces = type.GetInterfaces()
                    .Where(i => i.IsGenericType && i.GetGenericTypeDefinition() == behaviorType);

                foreach (var iface in behaviorInterfaces)
                {
                    if (type.IsGenericTypeDefinition)
                        services.AddScoped(iface.GetGenericTypeDefinition(), type);
                    else
                        services.AddScoped(iface, type);
                }
            }

            return services;
        }
    }
}
