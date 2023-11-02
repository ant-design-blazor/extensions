using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;

namespace AntDesign.Extensions;
public partial class TryBlazor
{
    [Inject]
    internal ICompileService BlazorCompileService { get; set; } = default!;

    #region DefaultValue

    [Parameter]
    public string Style { get; set; } = "";

    [Parameter]
    public string Language { get; set; } = MonacoEditorLanguages.Razor;

    [Parameter] 
    public object Options { get; set; } = new { minimap = new { enabled = true } };

    /// <summary>
    /// default code value
    /// </summary>
    [Parameter]
    public string DefaultValue { get; set; } = DefaultVal;

    private const string DefaultVal = """
<h1>Counter</h1>

<p role="status">Current count: @currentCount</p>

<button class="btn btn-primary" @onclick="IncrementCount">Click me</button>

@code {
    private int currentCount = 0;

    private void IncrementCount()
    {
        currentCount++;
    }
}
""";

    /// <summary>
    /// Whether to render the RenderFragment of compilation results 
    /// <para>
    /// if false, the RenderFragment can be access from <see cref="CompileResult"/>
    /// </para>
    /// </summary>
    [Parameter]
    public bool DefaultRender { get; set; } = true;

    /// <summary>
    /// Is an exception thrown when there is a compilation error.
    /// <para>
    /// if false, the error can be access from <see cref="CompileError"/>
    /// </para>
    /// </summary>
    [Parameter]
    public bool ThrowError { get; set; } = false;

    [Parameter]
    public EventCallback OnCompile { get; set; }

    #endregion

    StandaloneCodeEditor? _editor;

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            await _editor!.IsReady
                .ContinueWith(ready =>
                    ready.Result
                    ? _editor!.SetValue(DefaultValue)
                    : Task.CompletedTask
                );
        }
    }

    public RenderFragment? CompileResult { get; private set; }
    public Exception? CompileError { get; private set; }
    public List<CompileLog> Logs { get; } = new();

    private bool _isCompiling =false;

    public async Task RunCode()
    {
        _isCompiling = true;
        Logs.Clear();
        try
        {
            var code = await _editor!.GetValue();
            var codeInfos = new List<CodeInfo>()
            {
                new (code, "ComponentName.razor")
            };
            var compileResult =
                await BlazorCompileService.Compile(codeInfos);

            var type = compileResult.GetBlazorType();
            Logs.AddRange(compileResult.CompileLogs);
            if (type != null)
            {
                Logs.Add(new CompileLog(Content: "Render Blazor Component..."));
                CompileResult = builder =>
                {
                    builder.OpenComponent(0, type);
                    builder.CloseComponent();
                };
                StateHasChanged();
            }
            else
            {
                Logs.Add(new CompileLog(Content: "Blazor Component not found"));
                CompileResult = null;
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
            CompileError = e;
        }

        if (OnCompile.HasDelegate)
        {
            await OnCompile.InvokeAsync();
        }

        _isCompiling = false;
    }
}
