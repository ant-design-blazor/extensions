using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.JSInterop;

namespace AntDesign.Extensions;

public partial class MonacoEditor : IAsyncDisposable
{
    [Inject]
    internal IJSRuntime JsRuntime { get; set; } = default!;
    [Inject]
    internal IConfiguration Configuration { get; set; } = default!;

    [Parameter] 
    public string Height { get; set; } = "500px";

    [Parameter]
    public string Style { get; set; } = "";
    [Parameter]
    public string Class { get; set; } = "";

    [Parameter] 
    public string Theme { get; set; } = MonacoEditorThemes.Vs;

    [Parameter]
    public string Language { get; set; } = MonacoEditorLanguages.Plaintext;

    [Parameter]
    public object Options { get; set; } = new { };


    private readonly string _id = IdUtil.GenId();

    public IJSObjectReference? JsMirror { get; private set; }


    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            JsMirror = await JsRuntime.InvokeAsync<IJSInProcessObjectReference>(
                "window.AntDesign.ext.MonacoEditor.init", _id, 
                new
                {
                    theme = Theme,
                    language = Language,
                    othersOptions = Options
                });
        }
    }


    #region js mirror
    /// <summary>
    /// Replace the entire text buffer value contained in this model.
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public async Task SetValue(string value)
    {
        if (JsMirror != null)
        {
            await JsMirror.InvokeVoidAsync("setValue", value);
        }
    }

    /// <summary>
    /// Get the text stored in this model.
    /// </summary>
    /// <returns></returns>
    public async Task<string> GetValue()
    {
        if (JsMirror != null)
        {
            return await JsMirror.InvokeAsync<string>("getValue");
        }

        return "";
    }


    /// <summary>
    /// set editor language
    /// </summary>
    /// <param name="language"></param>
    /// <returns></returns>
    public async Task SetLanguage(string language)
    {
        ArgumentNullException.ThrowIfNull(language);
        if (JsMirror != null)
        {
            await JsMirror.InvokeVoidAsync("setLanguage", language);
        }
    }

    /// <summary>
    /// set editor theme
    /// </summary>
    /// <param name="themeName"></param>
    /// <returns></returns>
    public async Task SetTheme(string themeName)
    {
        ArgumentNullException.ThrowIfNull(themeName);
        if (JsMirror != null)
        {
            await JsMirror.InvokeVoidAsync("setTheme", themeName);
        }
    }

    #endregion


    public async ValueTask DisposeAsync()
    {
        if (JsMirror != null)
        {
            await JsMirror.InvokeVoidAsync("dispose");
            await JsMirror.DisposeAsync();
        }
        GC.SuppressFinalize(this);
    }
}
