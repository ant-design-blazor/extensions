﻿using System.Reflection;

namespace AntDesign.Extensions.Samples.Pages;

public partial class Index
{
    StandaloneCodeEditor? _editor;

    private readonly string _val = """
// This is a razor code snippet

@page "/";

<MonacoEditor />

@code{
    var a = 123;
};
""";

    private readonly List<string> _languages = new List<string>();
    private string _selectedLanguages = MonacoEditorLanguages.Typescript;

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
            await _editor!.IsReady.ContinueWith(ready =>
                ready.Result
                    ? _editor!.SetValue(_defaultVal)
                    : Task.CompletedTask
                    );
        }
    }
}
