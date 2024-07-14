
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.CompilerServices;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.Extensions.Options;
using OneOf;
using System.ComponentModel;

namespace AntDesign.Extensions;

public partial class Dialog : AntxDomComponentBase, IOkCancelComponent
{
    internal static string ClsPrefix = $"{Constants.ClsPrefix}-dialog";

    [Inject]
    private IPortalService PortalService { get; set; } = default!;

    #region mask
    [Parameter]
    public bool ShowMask { get; set; } = true;

    [Parameter]
    public bool MaskClosable { get; set; } = true;

    [Parameter]
    public ClassBuilder? MaskClass { get; set; }

    [Parameter]
    public StyleBuilder? MaskStyle { get; set; }

    #endregion

    [Parameter]
    [EditorRequired]
    public bool Visible { get; set; }

    [Parameter]
    public EventCallback<bool> VisibleChanged { get; set; }

    #region Header

    [Parameter]
    public string? Title { get; set; } = "";

    [Parameter]
    public RenderFragment? TitleTempalte { get; set; } = null;

    [Parameter]
    public ClassBuilder? HeaderClass { get; set; }
    [Parameter]
    public StyleBuilder? HeaderStyle { get; set; }

    #endregion

    #region body

    [Parameter]
    public RenderFragment? ChildContent { get; set; } = null;

    #endregion

    #region  Footer

    [Parameter]
    public RenderFragment? Footer { get; set; } = DefaultFooter.Instance;
    [Parameter]
    public ClassBuilder? FooterClass { get; set; }
    [Parameter]
    public StyleBuilder? FooterStyle { get; set; }

    [Parameter]
    public EventCallback<CancelEventArgs> OnOk { get; set; }
    
    [Parameter]
    public EventCallback OnCancel { get; set; }

    [Parameter]
    public OneOf<string, RenderFragment> OkText{ get; set; } = "OK";

    [Parameter]
    public OneOf<string, RenderFragment> CancelText { get; set; } = "Cancel";

    #endregion


    [Parameter]
    public DialogMode Mode { get; set; } = DialogMode.Modal;

    [Parameter]
    public bool Center { get; set; } = false;

    [Parameter]
    public int Top { get; set; } = 100;

    [Parameter]
    public int Width { get; set; } = 520;

    [Parameter]
    public bool DestroyOnClose { get; set; }

    [Parameter]
    public OneOf<string, ElementReference>? Container { get; set; } = "body";

    private DialogCoreOptions CreateOptions()
    {
        return new DialogCoreOptions
        {
            ClsPrefix = ClsPrefix,
            ShowMask = ShowMask,
            OnMaskClick = OnMaskClick,
            MaskClass = MaskClass,
            MaskStyle = MaskStyle,
            Header = CreateHeader,
            HeaderClass = HeaderClass,
            HeaderStyle = HeaderStyle,
            ContentClass = Class,
            ContentStyle = Style,
            Footer = Footer,
            FooterClass = FooterClass,
            FooterStyle = FooterStyle,
            Visible = Visible,
            Center = Center,
            Top = Top,
            Width = Width,
            DestroyOnClose = DestroyOnClose,
            Mode = Mode,
        };
    }

    protected void CreateHeader(RenderTreeBuilder builder)
    {
        if (Title == null && TitleTempalte == null)
        {
            return;
        }

        builder.OpenComponent<Header>(-5);
        if(TitleTempalte != null)
        {
            builder.AddAttribute(-4, nameof(Header.ChildContent), TitleTempalte);
        }
        else
        {
            builder.AddAttribute(-3, nameof(Header.ChildContent), (RenderFragment)((_builder) =>
            {
                _builder.AddMarkupContent(-2, Title);
            }
            ));
        }
        builder.AddAttribute(-1, nameof(Header.OnCancel), RuntimeHelpers.TypeCheck
            (
            EventCallback.Factory.Create(this, OnCancelBtnClick)
            )
        );
        builder.CloseComponent();
    }


    internal async Task OnMaskClick()
    {
        if (MaskClosable)
        {
            await OnCancelBtnClick();
        }

    }
    internal async Task OnOkBtnClick()
    {
        CancelEventArgs e = new CancelEventArgs();
        if (OnOk.HasDelegate)
        {
            await OnOk.InvokeAsync(e);
        }
        if (e.Cancel)
        {
            return;
        }
        if (VisibleChanged.HasDelegate)
        {
            await VisibleChanged.InvokeAsync(false);
        }
    }

    internal async Task OnCancelBtnClick()
    {
        if (OnCancel.HasDelegate)
        {
            await OnCancel.InvokeAsync();
        }
        if (VisibleChanged.HasDelegate)
        {
            await VisibleChanged.InvokeAsync(false);
        }
    }

    Task IOkCancelComponent.OnOk()
    {
        return OnOkBtnClick();
    }

    Task IOkCancelComponent.OnCancel()
    {
        return OnCancelBtnClick();
    }


    private IAsyncDisposable? _portalSubscribe;
    private ElementReference _portalRef;
    private ClassBuilder _portalStyle = new();
    private bool _hasAdd;
    private bool _showDom;

    protected override void OnInitialized()
    {
        _portalStyle.AddIf("display: none", () => !_showDom);
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (Visible)
        {
            if (!_hasAdd)
            {
                _showDom = false;
                _hasAdd = true;
                await InvokeAsync(StateHasChanged);
                return;
            }
            if (!_showDom)
            {
                await MoveToContainer();
                _showDom = true;
                await InvokeAsync(StateHasChanged);
                return;
            }
        }
        else
        {
            if (_showDom)
            {
                _showDom = false;
                await InvokeAsync(StateHasChanged);
                return;

            }

            if (_hasAdd && DestroyOnClose)
            {
                _hasAdd = false;
                await InvokeAsync(StateHasChanged);
            }
        }
    }

    internal async Task MoveToContainer()
    {
        if (Mode == DialogMode.Normal)
        {
            return;
        }

        if (Container == null)
        {
            Container = "body";
        }
        _portalSubscribe = await PortalService.CreateAsync(_portalRef, Container.Value);
    }

    internal async Task RemoveDom()
    {
        if (_portalSubscribe != null)
        {
            await _portalSubscribe.DisposeAsync();
        }
    }

    protected override void Dispose(bool disposing)
    {
        _ = RemoveDom();
        base.Dispose(disposing);
    }
}
