using Microsoft.AspNetCore.Components;

namespace AntDesign.Extensions;
public class DialogCoreOptions
{
    public string ClsPrefix { get; set; } = $"{Constants.ClsPrefix}-dialog";

    public bool ShowMask { get; set; }

    public ClassBuilder? MaskClass { get; set; }

    public StyleBuilder? MaskStyle { get; set; }

    public Func<Task>? OnMaskClick { get; set; } = null;


    public RenderFragment? Header { get; set; } = null;

    public ClassBuilder? HeaderClass { get; set; }

    public StyleBuilder? HeaderStyle { get; set; }


    public ClassBuilder? ContentClass { get; set; }

    public StyleBuilder? ContentStyle { get; set; }


    public bool Resizable { get; set; } = true;


    public RenderFragment? Footer { get; set; } = null;

    public ClassBuilder? FooterClass { get; set; }

    public StyleBuilder? FooterStyle { get; set; }


    public bool Visible { get; set; }


    public bool Center { get; set; } = true;


    public int Top { get; set; }


    public int Width { get; set; }


    public bool DestroyOnClose { get; set; }


    public DialogMode Mode { get; set; }


    /// <summary>
    /// quick configuration of Header == null && Footer == null
    /// </summary>
    public bool IsPure { get; set; }

}
