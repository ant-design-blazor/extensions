using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.CompilerServices;
using Microsoft.AspNetCore.Components.Rendering;

namespace AntDesign.Extensions;

public abstract class DialogForm : DialogFormBase
{
    /// <summary>
    /// For use in WASM mode. Do not use this Server Mode!
    /// </summary>
    protected DialogForm(string? title = null) : base()
    {
        if (!string.IsNullOrWhiteSpace(title))
        {
            Title = title;
        }
    }


    protected DialogForm(IServiceProvider serviceProvider, string? title = "") : base(serviceProvider)
    {
        if (!string.IsNullOrWhiteSpace(title))
        {
            Title = title;
        }
    }


    public string Title { get; set; } = "";

    public bool ShowMask { get; set; } = true;

    public bool MaskClosable { get; set; } = true;

    public bool ShowFooter { get; set; } = true;

    public DialogMode Mode { get; set; } = DialogMode.Modal;

    public bool Center { get; set; } = false;

    public int Top { get; set; } = 100;

    public int Width { get; set; } = 520;


    protected override async Task OnMaskClick()
    {
        if (MaskClosable)
        {
            await base.CloseAsync();
        }
    }

    protected override void CreateHeader(RenderTreeBuilder builder)
    {
        if (Title != null)
        {
            builder.OpenComponent<Header>(0);
            builder.AddAttribute(1, nameof(Header.Title), Title);
            builder.AddAttribute(2, nameof(Header.OnCancel), RuntimeHelpers.TypeCheck
                (
                EventCallback.Factory.Create(this, base.OnCancel)
                )
            );
            builder.CloseComponent();
        }
    }

    protected override void CreateFooter(RenderTreeBuilder builder)
    {
        if (ShowFooter)
        {
            int seq = 0;
            CreateCascadingValue(builder, seq++, seq++, this, seq++, true, seq++, (_builder) =>
            {
                _builder.OpenComponent<DefaultFooter>(seq++);
                _builder.CloseComponent();
            });
        }
    }

    protected static void CreateCascadingValue<TValue>(
        RenderTreeBuilder builder,
        int seq,
        int seqVal,
        TValue value,
        int seqIsFixed,
        bool argIsFixed,
        int seqChildContent,
        RenderFragment argChildContent)
    {
        builder.OpenComponent<CascadingValue<TValue>>(seq);
        builder.AddAttribute(seqVal, "Value", value);
        builder.AddAttribute(seqIsFixed, "IsFixed", argIsFixed);
        builder.AddAttribute(seqChildContent, "ChildContent", argChildContent);
        builder.CloseComponent();
    }

    protected override DialogCoreOptions CreateOptions()
    {
        var options = base.CreateOptions();
        options.ShowMask = ShowMask;
        options.Mode = Mode;
        options.Center = Center;
        options.Top = Top;
        options.Width = Width;
        options.DestroyOnClose = DestroyOnClose;

        return options;
    }

}

