using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace AntDesign.Extensions;
public class TryBlazorMultiFile : TryBlazorBase
{
    public TryBlazorMultiFile()
    {
        base.RenderTabs = true;
    }

    /// <summary>
    /// code file list
    /// </summary>
    [Parameter]
    public List<CodeFile> CodeFiles { get; set; } = new ();

    public override async Task SetParametersAsync(ParameterView parameters)
    {
        if (parameters.TryGetValue<List<CodeFile>>(nameof(CodeFiles), out var cdeList))
        {
            base.CodeFileList = cdeList;
        }
        await base.SetParametersAsync(parameters);
    }

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        base.BuildRenderTree(builder);
    }
}
