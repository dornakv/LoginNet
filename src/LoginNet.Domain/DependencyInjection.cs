using Microsoft.Extensions.DependencyInjection;
using LoginNet.Domain.Interfaces;
using LoginNet.Domain.Services;

namespace LoginNet.Domain
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddDomainServices(this IServiceCollection services)
        {
            services.AddScoped<IRoleDomainService, RoleDomainService>();
            return services;
        }
    }
}
