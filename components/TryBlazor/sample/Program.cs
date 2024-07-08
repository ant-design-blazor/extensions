using System.Reflection;
using AntDesign;
using AntDesign.Extensions;
using AntDesign.Extensions.Samples;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

// 获取 antd 的所有组件
//var components = typeof(ButtonType).Assembly
//    .ExportedTypes
//    .Where(x => x.IsSubclassOf(typeof(ComponentBase)))
//    .Select(x => x.Name)
//    .Select(x => x.Split("`")[0])
//    .Where(x => !x.EndsWith("Base"))
//    .Distinct()
//    .Select(x=> $"'{x}'")
//    .ToList();
//var s = string.Join("," + Environment.NewLine, components);
//Console.WriteLine(s);

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
