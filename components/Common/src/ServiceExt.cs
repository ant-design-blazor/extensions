
using Microsoft.Extensions.DependencyInjection;

namespace AntDesign.Extensions;


public static class ServiceExt
{
    public static IServiceCollection AddPortal(this IServiceCollection services)
    {
        services.AddScoped<IPortalService, PortalService>();
        return services;
    }

    public static IServiceCollection AddContainerRegistry(this IServiceCollection services)
    {
        services.AddScoped<IContainerRegistry, ContainerRegistry>();
        return services;
    }
}
