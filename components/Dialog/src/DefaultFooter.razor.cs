
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components;

namespace AntDesign.Extensions;
public partial class DefaultFooter
{
    internal static RenderFragment Instance = builder => {
        builder.OpenComponent<DefaultFooter>(0);
        builder.CloseComponent();
    };

    [CascadingParameter]
    [EditorRequired]
    public IOkCancelComponent OkCancelComponent { get; set; } = default!;

    protected virtual async Task HandleCancel(MouseEventArgs e)
    {
        await OkCancelComponent.OnCancel();
    }

    protected virtual async Task HandleOk(MouseEventArgs e)
    {
        await OkCancelComponent.OnOk();
    }
}
