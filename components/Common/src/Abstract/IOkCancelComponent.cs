using Microsoft.AspNetCore.Components;
using OneOf;

namespace AntDesign.Extensions;

public interface IOkCancelComponent
{
    Task OnOk();
    Task OnCancel();

    OneOf<string, RenderFragment> OkText { get; set; }

    OneOf<string, RenderFragment> CancelText { get; set; }
}
