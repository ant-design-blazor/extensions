using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AntDesign.Extensions;
public partial class TryBlazor: TryBlazorBase
{
    public TryBlazor()
    {
        base.RenderTabs = false;
    }

    [Parameter] 
    public string? DefaultValue { get; set; } = "";

    /// <summary>
    /// code file 
    /// </summary>
    [Parameter]
    public CodeFile? CodeFile { get; set; } = default!;

    public override async Task SetParametersAsync(ParameterView parameters)
    {
        List<CodeFile> codeFiles = new List<CodeFile>();
        if (parameters.TryGetValue<CodeFile>(nameof(CodeFile), out var codeFile))
        {
            codeFiles.Add(codeFile);
        }
        else if (parameters.TryGetValue<string>(nameof(DefaultValue), out var val))
        {
            codeFiles.Add(new CodeFile()
            {
                Code = val,
                FileName = "App.razor"
            });
        }
        else
        {
            codeFiles.Add(new CodeFile()
            {
                Code = "",
                FileName = "App.razor"
            });
        }
        base.CodeFileList = codeFiles;
        await base.SetParametersAsync(parameters);
    }

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        base.BuildRenderTree(builder);
    }
}
