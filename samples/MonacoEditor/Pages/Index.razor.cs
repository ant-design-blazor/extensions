using System.Reflection;

namespace AntDesign.Extensions.Samples.Pages;

public partial class Index
{
    MonacoEditor? _editor;

    private readonly string _val = """
// please change the language to "razor"
@page "/";

<MonacoEditor />

@code{
    var a = 123;
};
""";

    private readonly List<string> _languages = new List<string>();
    public Index()
    {
        var t = typeof(MonacoEditorLanguages);
        BindingFlags flag = BindingFlags.Static | BindingFlags.Public;
        var fs = t.GetFields(flag);
        var ins = new MonacoEditorLanguages();
        foreach (var f in fs)
        {
            var v = f.GetValue(ins);
            _languages.Add((string)v!);
        }
    }


    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
             await Task.Delay(1000).ContinueWith( (s) => _editor!.SetValue(_defaultVal));
        }
    }
}
