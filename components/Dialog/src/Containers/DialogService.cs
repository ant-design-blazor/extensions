
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Routing;

namespace AntDesign.Extensions;

public sealed class DialogService : ContainerServiceBase, IDialogService
{
    internal const string Name = "dialog";

    private readonly NavigationManager _navigationManager;

    public DialogService(IContainerRegistry containerDispatcher, NavigationManager navigationManager) : base(containerDispatcher)
    {
        _navigationManager = navigationManager;
    }

    protected override string GetName() => Name;


    public IDialogRef Create(DialogOptions dialogOptions)
    {
        return InternalCreate(dialogOptions, false);
    }

    public IDialogRef CreateAndOpen(DialogOptions dialogOptions)
    {
        return InternalCreate(dialogOptions, true);
    }

    private IDialogRef InternalCreate(DialogOptions dialogOptions, bool visible)
    {
        dialogOptions.Visible = visible;
        if (!dialogOptions.Opened)
        {
            dialogOptions.Opened =true;

            var oldOnOk = dialogOptions.OnOk;
            dialogOptions.OnOk = async (e) =>
            {
                if (oldOnOk != null) 
                {
                    await oldOnOk(e);
                }
                if (!e.Cancel && dialogOptions.Visible)
                {
                    dialogOptions.Visible = false;
                    AddOrUpdateOptions(dialogOptions);
                }
            };

            var oldOnCancel = dialogOptions.OnCancel;
            dialogOptions.OnCancel = async () =>
            {
                if (oldOnCancel != null)
                {
                    await oldOnCancel();
                }
                if (dialogOptions.Visible)
                {
                    dialogOptions.Visible = false;
                    AddOrUpdateOptions(dialogOptions);
                }
            };
        }

        AddOrUpdateOptions(dialogOptions);
        var dialogRef = new DialogRef(this, dialogOptions, _navigationManager);
        return dialogRef;
    }
}
