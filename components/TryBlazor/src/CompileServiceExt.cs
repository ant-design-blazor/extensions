using Microsoft.Extensions.DependencyInjection;

namespace AntDesign.Extensions;

public static class CompileServiceExt
{
    public static IServiceCollection AddBlazorCompileService
        (this IServiceCollection services, Action<BlazorCompileServiceOptions>? options = null)
    {
        services.AddScoped<ICompileService, BlazorCompileService>();

        if (options != null)
        {
            services.Configure<BlazorCompileServiceOptions>(options);
        }
        return services;
    }
}