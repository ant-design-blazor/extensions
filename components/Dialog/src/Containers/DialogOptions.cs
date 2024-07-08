using Microsoft.AspNetCore.Components;
using OneOf;
using System;
using System.ComponentModel;

namespace AntDesign.Extensions;
public class DialogOptions : FragmentOptions
{
    internal bool Opened { get; set; }

    #region mask

    public bool ShowMask { get; set; } = true;


    public bool MaskClosable { get; set; } = true;


    public ClassBuilder? MaskClass { get; set; }


    public StyleBuilder? MaskStyle { get; set; }

    #endregion

    public bool Visible { get; set; }

    public EventCallback<bool> VisibleChanged { get; set; }

    #region Header

    public string? Title { get; set; } = "";

    public ClassBuilder? HeaderClass { get; set; }

    public StyleBuilder? HeaderStyle { get; set; }

    #endregion

    #region body

    public OneOf<string, RenderFragment> ChildContent { get; set; } = "";

    #endregion

    #region  Footer

    public RenderFragment? Footer { get; set; } = DefaultFooter.Instance;

    public ClassBuilder? FooterClass { get; set; }

    public StyleBuilder? FooterStyle { get; set; }

    public Func<CancelEventArgs, Task>? OnOk { get; set; }


    public Func<Task>? OnCancel { get; set; }

    public OneOf<string, RenderFragment> OkText { get; set; } = "OK";

    public OneOf<string, RenderFragment> CancelText { get; set; } = "Cancel";

    #endregion

    public DialogMode Mode { get; set; } = DialogMode.Modal;

    public bool Center { get; set; } = false;

    public int Top { get; set; } = 100;

    public int Width { get; set; } = 520;

    public bool DestroyOnClose { get; set; }

    public OneOf<string, ElementReference>? Container { get; set; } = "body";
}
