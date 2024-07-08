namespace AntDesign.Extensions;
public sealed class DialogFormService : IDialogFormService
{
    private readonly IServiceProvider _serviceProvider;

    public DialogFormService(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

#pragma warning disable AntdExperimental

    public event Action<DialogFormBase>? OnOpen;
    public event Action<DialogFormBase>? OnClose;
    public event Func<Task>? OnStateHasChangedAsync;


    public DialogFormBase New<T>()
        where T : DialogFormBase
    {
        var form = (DialogFormBase)Activator.CreateInstance(typeof(T))!;
        form.SetServiceProvider(_serviceProvider, false);
        return form;
    }

    void IDialogFormService.Open(DialogFormBase dialogForm)
#pragma warning disable AntdExperimental
    {
        if (OnOpen != null)
        {
            OnOpen(dialogForm);
        }
    }
    void IDialogFormService.Close(DialogFormBase dialogForm)
    {
        if (OnClose != null)
        {
            OnClose(dialogForm);
        }
    }
    Task IDialogFormService.StateHasChangedAsync()
    {
        if (OnStateHasChangedAsync != null)
        {
            return OnStateHasChangedAsync();
        }
        return Task.CompletedTask;
    }
}
