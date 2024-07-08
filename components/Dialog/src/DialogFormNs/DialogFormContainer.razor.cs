using Microsoft.AspNetCore.Components;

namespace AntDesign.Extensions;

public partial class DialogFormContainer : IDisposable
{
    [Inject]
    internal IDialogFormService DialogFormService { get; set; } = default!;
    [Inject]
    internal IServiceProvider Services { get; set; } = default!;

    public DialogFormContainer()
    {
        _forms = new();
    }

    protected override void OnInitialized()
    {
        DialogFormService.OnOpen += DialogFormService_OnOpen;
        DialogFormService.OnClose += DialogFormService_OnClose;
        DialogFormService.OnStateHasChangedAsync += StateHasChangedAsync;
    }

    protected override void OnAfterRender(bool firstRender)
    {
        base.OnAfterRender(firstRender);
    }

    private Task StateHasChangedAsync()
    {
        return InvokeAsync(StateHasChanged);
    }

    private readonly HashSet<DialogFormBase> _forms;

    private void DialogFormService_OnClose(DialogFormBase obj)
    {
        if (_forms.Contains(obj))
        {
            _forms.Remove(obj);
            StateHasChangedAsync();
        }
    }

    private void DialogFormService_OnOpen(DialogFormBase obj)
    {
        if (_forms.Contains(obj))
        {
            return;
        }
        _forms.Add(obj);
        StateHasChangedAsync();
    }

    public void Dispose()
    {
        DialogFormService.OnOpen -= DialogFormService_OnOpen;
        DialogFormService.OnClose -= DialogFormService_OnClose;
        DialogFormService.OnStateHasChangedAsync -= StateHasChangedAsync;
    }
}
