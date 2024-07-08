
namespace AntDesign.Extensions;

public interface IDialogFormService
{
    internal event Action<DialogFormBase>? OnOpen;
    internal event Action<DialogFormBase>? OnClose;
    internal event Func<Task>? OnStateHasChangedAsync;

    DialogFormBase New<T>() where T : DialogFormBase;

    void Open(DialogFormBase dialogForm);
    void Close(DialogFormBase dialogForm);

    Task StateHasChangedAsync();
}