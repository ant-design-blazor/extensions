
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Routing;

namespace AntDesign.Extensions;

public sealed class DialogRef(DialogService DialogService, DialogOptions Options, NavigationManager NavigationManager) : IDialogRef
{
    public IDialogRef Open()
    {
        return DialogService.CreateAndOpen(Options);
    }

    public void Close()
    {
        Options.Visible = false;
        DialogService.AddOrUpdateOptions(Options);
    }

    public void Destroy()
    {
        Options.Visible = false;
        Options.DestroyOnClose = false;
        DialogService.RemoveOptions(Options);
    }

    private bool _onLocationChanged;
    public IDialogRef AutoRemoveOnLocationChanged()
    {
        if (!_onLocationChanged)
        {
            _onLocationChanged = true;
            NavigationManager.LocationChanged += OnLocationChanged;
        }
        return this;
    }

    private void OnLocationChanged(object? sender, LocationChangedEventArgs e)
    {
        NavigationManager.LocationChanged -= OnLocationChanged;
        this.Destroy();
    }
}
