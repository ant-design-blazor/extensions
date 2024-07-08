
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using AntDesign.Extensions;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.Services
    .AddAntDesign()
    .AddDialogForm()
    .AddPortal()
    .AddContainerRegistry();

var host = builder.Build();
host.Services.UseDialogForm();

await host.RunAsync();