using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;

namespace AntDesign.Extensions;
public partial class TryBlazorBase: ComponentBase
{
    public const string ClsPrefix = $"{Constants.ClsPrefix}-try-blazor";

    [Inject]
    internal ICompileService BlazorCompileService { get; set; } = default!;

    #region Parameters

    [Parameter]
    public string Style { get; set; } = "";

    [Parameter]
    public string Language { get; set; } = MonacoEditorLanguages.Razor;

    [Parameter]
    public object Options { get; set; } = new { minimap = new { enabled = true } };


    /// <summary>
    /// Whether to render the RenderFragment of compilation results 
    /// <para>
    /// if false, the RenderFragment can be access from <see cref="CompiledContent"/>
    /// </para>
    /// </summary>
    [Parameter]
    public bool DefaultRender { get; set; } = true;

    /// <summary>
    /// Is an exception thrown when there is a compilation error.
    /// <para>
    /// if false, the error can be access from <see cref="OnCompiled"/> event
    /// </para>
    /// </summary>
    [Parameter]
    public bool ThrowError { get; set; } = false;

    [Parameter]
    public EventCallback<List<CodeFile>> OnCompiling { get; set; }

    [Parameter]
    public EventCallback<CompileResult> OnCompiled { get; set; }

    [Parameter]
    public bool RenderTabs { get; set; }

    #endregion

    protected List<CodeFile> CodeFileList { get; set; } = new ();

    protected string ActiveKey = "";
    protected StandaloneCodeEditor? MonacoEditor;

    #region override

    protected override Task OnParametersSetAsync()
    {
        if (CodeFileList.Count > 0 && string.IsNullOrWhiteSpace(ActiveKey))
        {
            ActiveKey = CodeFileList[0].FileName;
        }

        return base.OnParametersSetAsync();
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            if (!string.IsNullOrWhiteSpace(ActiveKey))
            {
                var val = CodeFileList
                    .Find(x => x.FileName == ActiveKey)
                    !.Code;

                await MonacoEditor!.IsReady
                    .ContinueWith(ready =>
                        ready.Result
                            ? MonacoEditor!.SetValue(val)
                            : Task.CompletedTask
                    );
            }
        }
    }

    #endregion
    
    protected RenderFragment? CompiledContent { get; set; }

    protected List<CompileLog> Logs { get; } = new();

    protected bool IsCompiling = false;
    
    #region event handler

    protected async Task Compile()
    {
        IsCompiling = true;
        await InvokeAsync(StateHasChanged);
        await Task.Delay(10);
        Logs.Clear();
        CompileResult? compileResult = null;
        try
        {
            await SetCurrentCode();
            if (OnCompiling.HasDelegate)
            {
                await OnCompiling.InvokeAsync(CodeFileList);
            }
            compileResult =
                await BlazorCompileService.CompileAsync(CodeFileList);

            var type = compileResult.GetBlazorType();
            Logs.AddRange(compileResult.CompileLogs);
            if (type != null)
            {
                Logs.Add(new CompileLog(Content: "Render Blazor Component..."));
                CompiledContent = builder =>
                {
                    builder.OpenComponent(0, type);
                    builder.CloseComponent();
                };
                StateHasChanged();
            }
            else
            {
                Logs.Add(new CompileLog(Content: "Blazor Component not found"));
                CompiledContent = null;
            }
        }
        catch (Exception e)
        {
            Logs.Add(new CompileLog(e.Message, LogLevel.Error));
            if (e.StackTrace != null)
            {
                Logs.Add(new CompileLog(e.StackTrace, LogLevel.Error));
            }

            if (ThrowError)
            {
                throw;
            }

            compileResult = new CompileResult()
            {
                Error = e
            };
        }

        if (OnCompiled.HasDelegate)
        {
            await OnCompiled.InvokeAsync(compileResult);
        }

        IsCompiling = false;
    }

    protected async Task OnTabChange(string activeKey)
    {
        if (activeKey == ActiveKey)
        {
            return;
        }
        await SetCurrentCode();

        ActiveKey = activeKey;
        var codeInfo = CodeFileList.Find(x => x.FileName == ActiveKey)!;
        await MonacoEditor!.SetValue(codeInfo.Code);
        var language = codeInfo.CodeType switch
        {
            CodeType.Razor => "razor",
            CodeType.CSharp => "csharp",
            _ => "razor",
        };
        await MonacoEditor!.SetLanguage(language);
    }

    #endregion

    private async Task SetCurrentCode()
    {
        var curVal = await MonacoEditor!.GetValue();
        CodeFileList
            .Find(x => x.FileName == ActiveKey)
            !.Code = curVal;
    }
}