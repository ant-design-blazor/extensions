using Microsoft.AspNetCore.Components;

namespace AntDesign.Extensions;

public partial class Header
{
    [Parameter]
    public string ClsPrefix { get; set; } = Dialog.ClsPrefix;

    [Parameter]
    public string? Title { get; set; } = "";

    /// <summary>
    /// if tools is not null, OnCancel event will not take effect.
    /// </summary>
    [Parameter]
    public RenderFragment? Tools { get; set; }

    [Parameter]
    public EventCallback OnCancel { get; set; }

    private async Task OnCancelBtnClick()
    {
        if (OnCancel.HasDelegate)
        {
            await OnCancel.InvokeAsync();
        }
    }
}
