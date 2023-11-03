using System.Reflection;
using AntDesign.Extensions;
using AntDesign.Extensions.Samples;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

builder.Services.AddBlazorCompileService((options) =>
{
    options.RootNamespace = StringUtils.RootNsName;
    options.AdditionalImports = new[] { "@using AntDesign" };
    options.AdditionalAssemblies = new List<Assembly>()
    {
        typeof(AntDesign.Button).Assembly
    };
});

builder.Services.AddAntDesign();

await builder.Build().RunAsync();
