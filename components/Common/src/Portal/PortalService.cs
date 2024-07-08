using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using OneOf;

namespace AntDesign.Extensions;



public class PortalService : IPortalService
{
    private class PortalSubscription : IAsyncDisposable
    {
        private readonly ElementReference _eleRef;
        private readonly PortalService _portalService;

        public PortalSubscription(ElementReference eleRef, PortalService portalService)
        {
            _eleRef = eleRef;
            _portalService = portalService;
        }

        public async ValueTask DisposeAsync()
        {
            await _portalService.DeleteAsync(_eleRef);
        }
    }

    private readonly IJSRuntime JsRuntime;
    public PortalService(IJSRuntime jsRuntime)
    {
        JsRuntime = jsRuntime;
    }

    const string AppendJsFunc = "window.antd.ext.common.MoveEleTo";
    const string RemoveJsFunc = "window.antd.ext.common.RemoveEle";

    public async Task<IAsyncDisposable> CreateAsync(ElementReference eleRef, OneOf<string, ElementReference> container)
    {
        if (JsRuntime == null)
        {
            throw new NullReferenceException(nameof(JsRuntime));
        }
        if (container.IsT1)
        {
            await JsRuntime.InvokeVoidAsync(AppendJsFunc, eleRef, container.AsT1);
        }
        else
        {
            var selector = container.AsT0;
            if (string.IsNullOrEmpty(selector))
            {
                container = "body";
                selector = "body";
            }
            await JsRuntime.InvokeVoidAsync(AppendJsFunc, eleRef, selector);
        }

        return new PortalSubscription(eleRef, this);
    }

    public async Task DeleteAsync(ElementReference eleRef)
    {
        await JsRuntime.InvokeVoidAsync(RemoveJsFunc, eleRef);
    }
}
