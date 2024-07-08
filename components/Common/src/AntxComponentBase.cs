using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AntDesign.Extensions;
public abstract class AntxComponentBase : ComponentBase, IDisposable
{
    #region js

    [Inject]
    protected IJSRuntime Js { get; set; } = default!;

    protected IJSRuntime JsRuntime => Js;

    protected async Task<T> JsInvokeAsync<T>(string identifier, params object[] args)
    {
        return await Js.InvokeAsync<T>(identifier, args);
    }

    protected async Task JsInvokeAsync(string identifier, params object[] args)
    {
        await Js.InvokeVoidAsync(identifier, args);
    }

    #endregion

    #region StateHasChanged

    protected void InvokeStateHasChanged()
    {
        StateHasChanged();
    }

    protected async Task InvokeStateHasChangedAsync()
    {
        await InvokeAsync(() =>
        {
            if (!IsDisposed)
            {
                StateHasChanged();
            }
        });
    }

    #endregion


    #region IDisposable

    protected bool IsDisposed { get; private set; }

    protected virtual void Dispose(bool disposing)
    {
        if (IsDisposed)
        {
            return;
        }
        IsDisposed = true;
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    #endregion
}
