using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Options;
using Microsoft.JSInterop;
using OneOf;

namespace AntDesign.Extensions;

public enum DialogMode
{
    Dialog = 0,
    Modal = 1,
    Normal = 2,
}


public partial class DialogCore
{
    [Parameter]
    [EditorRequired]
    public DialogCoreOptions Options { get; set; } = new();

    [Parameter]
    public RenderFragment? ChildContent { get; set; } = null;

    public string Id { get; } = IdUtil.GenId();

    #region class style
    private ClassBuilder _maskClass = new();
    private StyleBuilder _maskStyle = new();

    private ClassBuilder _positionClass = new();
    private StyleBuilder _positionStyle = new();

    private ClassBuilder _mainClass = new();
    private ClassBuilder _mainStyle = new();

    private ClassBuilder _headerClass = new();
    private StyleBuilder _headerStyle = new();

    private ClassBuilder _contentClass = new();
    private StyleBuilder _contentStyle = new();


    private ClassBuilder _footerClass = new();
    private StyleBuilder _footerStyle = new();

    #endregion

    protected override Task OnInitializedAsync()
    {
        bool IsNotNullOrWhiteSpace(string val)
        {
            return !string.IsNullOrWhiteSpace(val);
        }

        _maskClass.Add($"{Options.ClsPrefix}-mask")
            .AddIf(Options.MaskClass?.ToString() ?? "", IsNotNullOrWhiteSpace);
        _maskStyle.AddIf(Options.MaskStyle?.ToString() ?? "", IsNotNullOrWhiteSpace);

        _positionClass.Add($"{Options.ClsPrefix}-position")
            .AddIf($"{Options.ClsPrefix}-center", () => Options.Center);
        _positionStyle.AddIf($"top: {Options.Top}px", () => !Options.Center)
            .AddIf($"width: {Options.Width}px", () => Options.Width > 0);

        _mainClass.Add($"{Options.ClsPrefix}-main")
            .AddIf($"{Options.ClsPrefix}-mode-normal", () => Options.Mode == DialogMode.Normal);
        _mainStyle.AddIf("resize: both; overflow: hidden;", () => Options.Resizable);

        _headerClass.Add($"{Options.ClsPrefix}-header")
          .AddIf(Options.HeaderClass?.ToString() ?? "", IsNotNullOrWhiteSpace);
        _headerStyle.AddIf(Options.HeaderStyle?.ToString() ?? "", IsNotNullOrWhiteSpace);


        _contentClass.Add($"{Options.ClsPrefix}-content")
          .AddIf(Options.ContentClass?.ToString() ?? "", IsNotNullOrWhiteSpace);
        _contentStyle.AddIf(Options.ContentStyle?.ToString() ?? "", IsNotNullOrWhiteSpace);


        _footerClass.Add($"{Options.ClsPrefix}-footer")
          .AddIf(Options.FooterClass?.ToString() ?? "", IsNotNullOrWhiteSpace);
        _footerStyle.AddIf(Options.FooterStyle?.ToString() ?? "", IsNotNullOrWhiteSpace);

        return Task.CompletedTask;
    }


    #region mask click check

    private bool _dialogMouseDown;
    private void OnDialogMouseDown()
    {
        _dialogMouseDown = true;
    }

    private async Task OnMaskMouseUp()
    {
        if (Options.Mode == DialogMode.Modal && !_dialogMouseDown && Options.OnMaskClick != null)
        {
            await Options.OnMaskClick();
        }
        _dialogMouseDown = false;
    }
    #endregion
}
