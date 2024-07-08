
using Microsoft.Extensions.DependencyInjection;

namespace AntDesign.Extensions;

/// <summary>
/// only used in wasm mode
/// </summary>
internal class AppUtil
{
#pragma warning disable CS8618 // 在退出构造函数时，不可为 null 的字段必须包含非 null 值。请考虑声明为可以为 null。
    public static IServiceProvider ServiceProvider { get; set; }
#pragma warning restore CS8618 // 在退出构造函数时，不可为 null 的字段必须包含非 null 值。请考虑声明为可以为 null。
}

public static class ServicesExtension
{
    public static IServiceCollection AddDialogForm(this IServiceCollection services)
    {
        services.AddScoped<IDialogFormService, DialogFormService>();
        services.AddScoped<IDialogService, DialogService>();
        return services;
    }


    public static IServiceProvider UseDialogForm(this IServiceProvider services)
    {
        AppUtil.ServiceProvider = services;
        return services;
    }
}
