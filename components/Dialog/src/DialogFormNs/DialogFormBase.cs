using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.Extensions.DependencyInjection;
using OneOf;
using System.ComponentModel;

namespace AntDesign.Extensions;


public abstract class DialogFormBase : IDisposable, IOkCancelComponent //, IComponent
{
    public const string Prefix = $"{Constants.ClsPrefix}-dialog";

    protected IServiceProvider ServiceProvider { get; private set; }

    private IDialogFormService _dialogFormService;

    /// <summary>
    /// For use in WASM mode. Do not use this Server Mode!
    /// </summary>
    
    protected DialogFormBase() : this(AppUtil.ServiceProvider)
    {
    }

#pragma warning disable CS8618 // 在退出构造函数时，不可为 null 的字段必须包含非 null 值。请考虑声明为可以为 null。
    protected DialogFormBase(IServiceProvider serviceProvider)
#pragma warning restore CS8618 // 在退出构造函数时，不可为 null 的字段必须包含非 null 值。请考虑声明为可以为 null。
    {
        InitComponent();
        RenderFragment = builder => BuildRenderTree(builder);
        _header = builder => CreateHeader(builder);
        _footer = builder => CreateFooter(builder);

        SetServiceProvider(serviceProvider, true);
    }

    internal void SetServiceProvider(IServiceProvider serviceProvider, bool fromCtor)
    {
        if (fromCtor && serviceProvider is null)
        {
            return;
        }

        ServiceProvider = serviceProvider;
        _dialogFormService = ServiceProvider.GetRequiredService<IDialogFormService>();
    }


    protected virtual void InitComponent()
    {
    }


    private RenderFragment _header { get; init; }
    private RenderFragment _footer { get; init; }

    internal RenderFragment RenderFragment { get; init; }


    internal Func<Task> OnMaskClickEvent => OnMaskClick;

    internal DialogCoreOptions GetOptions()
    {
        var options = CreateOptions();
        options.Header = _header;
        options.Footer = _footer;
        options.OnMaskClick = OnMaskClick;
        options.Visible = true;

        return options;
    }


    /// <summary>
    /// for razor compile
    /// </summary>
    /// <param name="builder"></param>
    protected virtual void BuildRenderTree(RenderTreeBuilder builder)
    {
    }

    protected virtual void CreateHeader(RenderTreeBuilder builder)
    {

    }

    protected virtual void CreateFooter(RenderTreeBuilder builder)
    {

    }

    protected virtual DialogCoreOptions CreateOptions()
    {
        return new DialogCoreOptions
        {
            Visible = true,
            ShowMask = true,
            Resizable = true,
        };
    }

    protected virtual Task OnMaskClick()
    {
        return Task.CompletedTask;
    }

    #region paramter

    public bool DestroyOnClose { get; protected set; } = true;

    #endregion

    public bool Visible { get; private set; }

    public bool HasDestroyed { get; private set; } = true;

    #region show

    /// <summary>
    /// Trigger before displaying form. The display can be cancelled by CancelEventArgs.
    /// </summary>
    /// <param name="e"></param>
    /// <returns></returns>
    protected virtual Task OnShowingAsync(CancelEventArgs e)
    {
        return Task.CompletedTask;
    }

    /// <summary>
    /// Show form
    /// </summary>
    /// <returns></returns>  
    public async Task ShowAsync()
    {
        CancelEventArgs eventArgs = new CancelEventArgs();
        await OnShowingAsync(eventArgs);
        if (eventArgs.Cancel)
        {
            return;
        }

        if (!Visible)
        {
            Visible = true;
            if (HasDestroyed)
            {
                HasDestroyed = false;
                _dialogFormService.Open(this);
            }
            await StateHasChangedAsync();
        }
    }

    #endregion


    #region close

    /// <summary>
    /// Trigger before closing form. The close can be cancelled by CancelEventArgs
    /// </summary>
    /// <param name="e"></param>
    /// <returns></returns>
    protected virtual Task OnClosingAsync(CancelEventArgs e)
    {
        return Task.CompletedTask;
    }

    /// <summary>
    /// Trigger before the form destroying (removing) from DOM. The destroy Can be cancelled by CancelEventArgs
    /// </summary>
    /// <param name="e"></param>
    /// <returns></returns>
    protected virtual Task OnDestroyingAsync()
    {
        return Task.CompletedTask;
    }


    /// <summary>
    /// Close form.
    /// <para>
    ///     If you want to call the <b>base.CloseAsync</b> in the component, be sure to use <b>pure event handlers</b>! Just like:
    ///     <code>
    ///         @onclick="EventUtil.AsNonRenderingEventHandler(async => { await base.CloseAsync(); })"
    ///     </code>
    /// </para>
    /// <para>
    ///     It's in <b>EventUtil</b> class and is provided by SteveSandersonMs
    /// </para>
    /// </summary>
    /// <returns></returns>
    public async Task CloseAsync()
    {
        CancelEventArgs eventArgs = new CancelEventArgs(false);
        await OnClosingAsync(eventArgs);

        if (eventArgs.Cancel)
        {
            return;
        }

        if (Visible)
        {
            Visible = false;
            await StateHasChangedAsync();

            if (DestroyOnClose && !HasDestroyed)
            {
                HasDestroyed = true;
                await OnDestroyingAsync();

                if (eventArgs.Cancel)
                {
                    return;
                }
                _dialogFormService.Close(this);
            }
        }
    }

    #endregion

    protected Task StateHasChangedAsync()
    {
        return _dialogFormService.StateHasChangedAsync();
    }


    #region dispose

    /// <summary>
    /// Has the object been released
    /// </summary>
    protected bool IsDisposed { get; private set; }

    /// <summary>
    /// If rewriting, be sure to base.Dispose Call this function
    /// </summary>
    /// <param name="disposing"></param>
    protected virtual void Dispose(bool disposing)
    {
        if (!IsDisposed && disposing)
        {
            _dialogFormService.Close(this);
        }
    }

    /// <summary>
    /// dispose the form
    /// </summary>
    public void Dispose()
    {
        Dispose(true);

        GC.SuppressFinalize(this);
    }

    ~DialogFormBase()
    {
        // Finalizer calls Dispose(false)
        Dispose(false);
    }

    #endregion

    public OneOf<string, RenderFragment> OkText { get; set; } = "OK";
    public OneOf<string, RenderFragment> CancelText { get; set; } = "Cancel";

    public Task OnOk()
    {
        return this.CloseAsync();
    }

    public Task OnCancel()
    {
        return this.CloseAsync();
    }

    #region IComponent

    //private object? _registeredIdentifier;
    //private bool? _registeredIsDefaultContent;
    //private SectionRegistry _registry = default!;

    //void IComponent.Attach(RenderHandle renderHandle)
    //{
    //    _registry = renderHandle.Dispatcher.SectionRegistry;
    //}

    //Task IComponent.SetParametersAsync(ParameterView parameters)
    //{
    //    // We are not using parameters.SetParameterProperties(this)
    //    // because IsDefaultContent is internal property and not a parameter
    //    SetParameterValues(parameters);
    //    object? identifier;

    //    if (SectionName is not null && SectionId is not null)
    //    {
    //        throw new InvalidOperationException($"{nameof(SectionContent)} requires that '{nameof(SectionName)}' and '{nameof(SectionId)}' cannot both have non-null values.");
    //    }
    //    else if (SectionName is not null)
    //    {
    //        identifier = SectionName;
    //    }
    //    else if (SectionId is not null)
    //    {
    //        identifier = SectionId;
    //    }
    //    else
    //    {
    //        throw new InvalidOperationException($"{nameof(SectionContent)} requires a non-null value either for '{nameof(SectionName)}' or '{nameof(SectionId)}'.");
    //    }

    //    if (!object.Equals(identifier, _registeredIdentifier) || IsDefaultContent != _registeredIsDefaultContent)
    //    {
    //        if (_registeredIdentifier is not null)
    //        {
    //            _registry.RemoveProvider(_registeredIdentifier, this);
    //        }

    //        _registry.AddProvider(identifier, this, IsDefaultContent);
    //        _registeredIdentifier = identifier;
    //        _registeredIsDefaultContent = IsDefaultContent;
    //    }

    //    _registry.NotifyContentProviderChanged(identifier, this);

    //    return Task.CompletedTask;
    //}

    //private void SetParameterValues(in ParameterView parameters)
    //{
    //    foreach (var param in parameters)
    //    {
    //        switch (param.Name)
    //        {
    //            case nameof(SectionContent.SectionName):
    //                SectionName = (string)param.Value;
    //                break;
    //            case nameof(SectionContent.SectionId):
    //                SectionId = param.Value;
    //                break;
    //            case nameof(SectionContent.IsDefaultContent):
    //                IsDefaultContent = (bool)param.Value;
    //                break;
    //            case nameof(SectionContent.ChildContent):
    //                ChildContent = (RenderFragment)param.Value;
    //                break;
    //            default:
    //                throw new ArgumentException($"Unknown parameter '{param.Name}'");
    //        }
    //    }
    //}

    #endregion
}

