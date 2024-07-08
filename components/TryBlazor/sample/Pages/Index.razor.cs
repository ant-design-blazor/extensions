using Microsoft.AspNetCore.Components;

namespace AntDesign.Extensions.Samples.Pages;

public partial class Index
{
    private readonly List<CodeFile> _codeFiles = new List<CodeFile>();
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

    public Index()
    {
        var razorCodeInfo = new CodeFile()
        {
            Code = """
<h1>Counter</h1>

<p role="status">Current count: @currentCount</p>

<button class="btn btn-primary" @onclick="IncrementCount">Click me</button>
""",
            FileName = "Counter.razor"
        };
        var csharpCodeInfo = new CodeFile()
        {
            Code = """
public partial class Counter
{
    private int currentCount = 0;

    private void IncrementCount()
    {
        currentCount++;
    }
}
""",
            FileName = "Counter.razor.cs"
        };
        _codeFiles.Add(razorCodeInfo);
        _codeFiles.Add(csharpCodeInfo);
    }

    TryBlazor? _tryBlazor;
    TryBlazorMultiFile? _tryBlazorMultiFile;

    private string _logs = "";
    private void OnCompiled(CompileResult compileResult)
    {
        if (_tryBlazor != null)
        {
            var logs = compileResult.CompileLogs;
            _logs = string.Join("\r\n", logs);
        }
    }

    private RenderFragment? _compileContent = null;
    private void OnMultiFileCompiled(CompileResult compileResult)
    {
        if (_tryBlazorMultiFile != null)
        {
            _compileContent = compileResult.GetBlazorComponent();
            var logs = compileResult.CompileLogs;
            _logs = string.Join("\r\n", logs);
        }
    }
}
