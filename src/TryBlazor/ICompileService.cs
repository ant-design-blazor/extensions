using Microsoft.CodeAnalysis;

namespace AntDesign.Extensions;

public interface ICompileService
{
    Task<CompileResult> Compile(
        List<CodeInfo> codeInfos,
        string assemblyName = nameof(BlazorCompileService),
        OptimizationLevel optimizationLevel = OptimizationLevel.Release
    );
}