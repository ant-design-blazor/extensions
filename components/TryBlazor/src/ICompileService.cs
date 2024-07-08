using Microsoft.CodeAnalysis;
using System.IO;

namespace AntDesign.Extensions;

public interface ICompileService
{
    /// <summary>
    /// load reference from static file. The dll must in _framework directory
    /// </summary>
    /// <param name="name"></param>
    /// <param name="isBlazorComponent"></param>
    /// <returns></returns>
    Task LoadReference(string name, bool isBlazorComponent = false);

    /// <summary>
    /// load reference from dll file stream.
    /// </summary>
    /// <param name="stream"></param>
    /// <param name="isBlazorComponent"></param>
    /// <returns></returns>
    Task LoadReference(Stream stream, bool isBlazorComponent = false);

    Task<CompileResult> CompileAsync(
        List<CodeFile> codeInfos,
        string assemblyName = nameof(BlazorCompileService),
        OptimizationLevel optimizationLevel = OptimizationLevel.Release
    );

    Task<CompileStreamResult> CompileToStreamAsync(
        List<CodeFile> codeInfos,
        string assemblyName = nameof(BlazorCompileService),
        OptimizationLevel optimizationLevel = OptimizationLevel.Release
    );
}