using System.Reflection;

namespace AntDesign.Extensions;

public class BlazorCompileServiceOptions
{
    /// <summary>
    /// compile assembly root name space
    /// </summary>
    public string RootNamespace { get; set; } = "TryBlazor";

    /// <summary>
    /// a item such as: "@using System"
    /// </summary>
    public string[] AdditionalImports { get; set; } = Array.Empty<string>();

    /// <summary>
    /// 
    /// </summary>
    public List<Assembly> AdditionalAssemblies { get; set; } = new ();


    public bool LazyInitRazorProjectEngine = true;


    public const string DefaultImportsStr = """
@using System.Net.Http
@using System.Net.Http.Json
@using Microsoft.AspNetCore.Components.Forms
@using Microsoft.AspNetCore.Components.Routing
@using Microsoft.AspNetCore.Components.Web
@using Microsoft.AspNetCore.Components.Web.Virtualization
@using Microsoft.AspNetCore.Components.WebAssembly.Http
@using Microsoft.JSInterop
""";

    private string[]? _defaultImports = null;
    /// <summary>
    /// a item such as: "@using System"
    /// </summary>
    internal string[] DefaultImports {
        get
        {
            if (_defaultImports == null)
            {
                _defaultImports = new string[AdditionalImports.Length + 1];
                _defaultImports[0] = DefaultImportsStr;
                Array.Copy(
                    AdditionalImports,
                    0, 
                    _defaultImports,
                    1,
                    AdditionalImports.Length);
            }

            return _defaultImports;
        }
    }
}