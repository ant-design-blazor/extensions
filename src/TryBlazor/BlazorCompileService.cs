using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.IO;
using System.IO.Pipelines;
using System.Net.Http.Json;
using System.Reflection;
using System.Runtime;
using System.Runtime.Loader;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.WebAssembly.Services;
using Microsoft.AspNetCore.Razor.Language;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Emit;
using Microsoft.CodeAnalysis.Razor;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.JSInterop;

namespace AntDesign.Extensions;

public class BlazorCompileService : ICompileService
{
    private const string WorkSpace = "/WorkSpace";

    private readonly ILogger<BlazorCompileService> _logger;
    private readonly BlazorCompileServiceOptions _blazorCompileServiceOptions;
    private readonly HttpClient _http;
    private readonly NavigationManager _uriHelper;


    private readonly List<MetadataReference> _references = new();
    private RazorProjectEngine? _projectEngine = null;
    private Task? _initTask = null;

    private readonly RazorProjectFileSystem _fileSystem = new VirtualRazorProjectFileSystem();
    private readonly RazorConfiguration _config = RazorConfiguration.Create(
                        RazorLanguageVersion.Latest,
                        configurationName: "Blazor",
                        extensions: Array.Empty<RazorExtension>()
                        );

    /// <summary>
    /// 0: not init
    /// 1: initing
    /// 2: inited
    /// </summary>
    private volatile int _projectEngineStatus = 0;

    public BlazorCompileService(
        HttpClient http,
        NavigationManager uriHelper,
        IOptions<BlazorCompileServiceOptions> options, ILogger<BlazorCompileService> logger)
    {
        _http = http;
        _uriHelper = uriHelper;
        _logger = logger;
        _blazorCompileServiceOptions = options.Value;

        if (!_blazorCompileServiceOptions.LazyInitRazorProjectEngine)
        {
            _initTask = SafeInitEngine();
        }
    }

    #region init

    private async Task SafeInitEngine()
    {
        if (_projectEngineStatus == 2)
        {
            return;
        }

        if (Interlocked.CompareExchange(ref _projectEngineStatus, 1, 0) == 0)
        {
            await UnsafeInitEngine();
            Interlocked.Exchange(ref _projectEngineStatus, 2);

        }

        while (Interlocked.CompareExchange(ref _projectEngineStatus, 2, 2) == 1)
        {
            Thread.SpinWait(10);
        }
    }

    private async Task UnsafeInitEngine()
    {
        await LoadReferences();
        _logger.LogDebug("Create engine...");
        _projectEngine = CreateRazorProjectEngine(_references);
        _logger.LogDebug("Create success");
    }

    private async Task LoadReferences()
    {
        if (_references.Count > 0)
        {
            return;
        }

        var loadedAssembly = new HashSet<string>();
        // basic assemblies
        var basicReferenceAssemblyRoots = new[]
        {
            typeof(HttpClient).Assembly, // System.Net.Http
            typeof(HttpClientJsonExtensions).Assembly, // System.Net.Http.Json
            typeof(ComponentBase).Assembly, // Microsoft.AspNetCore.Components
            typeof(LazyAssemblyLoader).Assembly, // Microsoft.AspNetCore.Components.WebAssembly
            typeof(IJSRuntime).Assembly, // Microsoft.JSInterop
            typeof(RequiredAttribute).Assembly, // System.ComponentModel.Annotations
            typeof(IQueryable).Assembly, // System.Linq.Expressions
            typeof(AssemblyTargetedPatchBandAttribute).Assembly, // System.Private.CoreLib
            typeof(Console).Assembly, // System.Console
            typeof(Uri).Assembly, // System.Private.Uri
        };
        var basicNames = basicReferenceAssemblyRoots
            .SelectMany(assembly => assembly
                .GetReferencedAssemblies()
                .Concat(new[] { assembly.GetName() })
            )
            .Select(x => x.Name)
            .Distinct()
            .ToList();
        // loaded  additional assemblies
        var additionalNames = _blazorCompileServiceOptions
            .AdditionalAssemblies
            .SelectMany(assembly => assembly.GetReferencedAssemblies().Concat(new[] { assembly.GetName() }))
            .Select(x => x.Name)
            .ToHashSet();

        var tasks = new List<Task>();
        basicNames.AddRange(additionalNames);
        foreach (var name in basicNames.ToHashSet())
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                continue;
            }

            if (loadedAssembly.Contains(name))
            {
                continue;
            }
            loadedAssembly.Add(name);
            tasks.Add(LoadReference(name + ".dll"));
        }

        await Task.WhenAll(tasks);
    }


    public async Task LoadReference(string name, bool isBlazorComponent = false)
    {
        //_logger.LogInformation("LoadReference:{name}", name);
        if (!name.EndsWith(".dll"))
        {
            name += ".dll";
        }

        var url = _uriHelper.BaseUri + "_framework/" + name;
        var response = await this._http.SendAsync(new HttpRequestMessage(HttpMethod.Get, url));
        if (response.IsSuccessStatusCode)
        {
            var stream = await response.Content.ReadAsStreamAsync();
            await LoadReference(stream, isBlazorComponent);
            return;
        }
        if (name.EndsWith("resources.dll"))
        {
            _logger.LogWarning("not found resources: {url}", url);
            return;
        }
        _logger.LogError("resources load error: {StatusCode}, {response}", response.StatusCode, response);
        throw new Exception("LoadReference error: not found assembly resources: " + url);
    }

    public Task LoadReference(Stream stream, bool isBlazorComponent = false)
    {
        _references.Add(MetadataReference.CreateFromStream(stream));
        if (isBlazorComponent)
        {
            _projectEngine = CreateRazorProjectEngine(_references);
        }
        return Task.CompletedTask;
    }

    #endregion

    private RazorProjectEngine CreateRazorProjectEngine(IReadOnlyList<MetadataReference> references) =>
        RazorProjectEngine.Create(this._config, this._fileSystem, builder =>
        {
            builder.SetRootNamespace(_blazorCompileServiceOptions.RootNamespace);
            builder.AddDefaultImports(_blazorCompileServiceOptions.DefaultImports);

            CompilerFeatures.Register(builder);

            builder.Features.Add(new CompilationTagHelperFeature());
            builder.Features.Add(new DefaultMetadataReferenceFeature { References = references });
        });

    public async Task<CompileResult> CompileAsync(
        List<CodeFile> codeInfos,
        string assemblyName = nameof(BlazorCompileService),
        OptimizationLevel optimizationLevel = OptimizationLevel.Release
        )
    {
        var streamResult = await
            CompileToStreamAsync(
                codeInfos,
                assemblyName,
                optimizationLevel
                );
        var result = new CompileResult(streamResult.CompileLogs);
        if (streamResult.Stream != null)
        {
            var assembly = AppDomain.CurrentDomain.Load(streamResult.Stream.ToArray());
            result.Assembly = assembly;
        }

        return result;
    }

    public async Task<CompileStreamResult> CompileToStreamAsync(
     List<CodeFile> codeInfos,
     string assemblyName = nameof(BlazorCompileService),
     OptimizationLevel optimizationLevel = OptimizationLevel.Release
     )
    {
        if (codeInfos == null || codeInfos.Count == 0)
        {
            throw new ArgumentException(nameof(codeInfos) + "cannot be null or empty");
        }

        var logs = new List<CompileLog>();
        var result = new CompileStreamResult(logs);
        await (_initTask ??= SafeInitEngine());
        Debug.Assert(_projectEngine != null);

        logs.Add(new CompileLog(Content: "begin compile..."));

        var syntaxTrees = new List<SyntaxTree>();
        foreach (CodeFile codeInfo in codeInfos)
        {
            var csharpCode = codeInfo.Code;
            if (codeInfo.CodeType == CodeType.Razor)
            {
                var (code, diagnostics)
                    = CompileRazorToCSharp(codeInfo, logs);
                if (DiagnosticHasError(diagnostics, logs))
                {
                    return result;
                }
                csharpCode = code;
            }
            else
            {
                // else must be c#, Otherwise, an exception will be thrown (codeFile.CodeType's getter)

                csharpCode = csharpCode.TrimStart();
                if (!csharpCode.StartsWith("namespace "))
                {
                    var ns = $"namespace {_blazorCompileServiceOptions.RootNamespace}; {Environment.NewLine}";
                    csharpCode = ns + csharpCode;
                }
            }

            logs.Add(new CompileLog(Content: "generate csharp syntax tree"));

            var syntaxTree = CSharpSyntaxTree
                .ParseText(csharpCode,
                    new CSharpParseOptions(LanguageVersion.Preview)
                    );
            if (DiagnosticHasError(syntaxTree.GetDiagnostics(), logs))
            {
                return result;
            }
            syntaxTrees.Add(syntaxTree);
        }


        var compilation = CSharpCompilation
            .Create(
                assemblyName,
                new List<SyntaxTree>(),
                new List<MetadataReference>(),
                new CSharpCompilationOptions(
                    OutputKind.DynamicallyLinkedLibrary,
                    optimizationLevel: optimizationLevel,
                    specificDiagnosticOptions: new[]
                    {
                        new KeyValuePair<string, ReportDiagnostic>("CS1701", ReportDiagnostic.Suppress),
                        // CS8019: 不需要的 using 指令
                        new KeyValuePair<string, ReportDiagnostic>("CS8019", ReportDiagnostic.Suppress),
                    }
                )
            );

        var (stream, _) = CompileCSharpToStream(
            compilation,
            syntaxTrees,
            _references,
            logs
        );

        result.Stream = stream;
        logs.Add(new CompileLog(
            $"end compile {assemblyName}, " +
            $"result.Stream is null:{result.Stream == null}")
        );
        return result;

        // two phase compilation
        // https://github.com/dotnet/razor/blob/6a5ccd98416f265b5c6ab8d06786e1ed19861363/src/Shared/Microsoft.AspNetCore.Razor.Test.Common/Language/IntegrationTests/RazorIntegrationTestBase.cs#L199 
    }


    private (MemoryStream?, CSharpCompilation) CompileCSharpToStream(
        CSharpCompilation baseCompilation,
        List<SyntaxTree> syntaxTrees,
        List<MetadataReference> references,
        List<CompileLog> logs)
    {
        var compilation = baseCompilation
            .AddSyntaxTrees(syntaxTrees)
            .AddReferences(references);

        var stream = new MemoryStream();
        EmitResult emitResult = compilation.Emit(stream);
        if (DiagnosticHasError(compilation.GetDiagnostics(), logs))
        {
            return (null, compilation);
        }

        if (!emitResult.Success)
        {
            logs.Add(new CompileLog("Compilation error", LogLevel.Error));
            return (null, compilation);
        }

        logs.Add(new CompileLog(Content: "CompileToAssembly success!"));

        stream.Seek(0, SeekOrigin.Begin);
        return (stream, compilation);
    }


    private (string, IReadOnlyList<RazorDiagnostic>) CompileRazorToCSharp(CodeFile codeFile, List<CompileLog> logs)
    {
        Debug.Assert(_projectEngine != null);

        logs.Add(new CompileLog(Content: $"begin compile razor {codeFile.FileName}"));

        var file = new VirtualProjectItem(
            codeFile.Code,
            WorkSpace,
            $"{WorkSpace}/{codeFile.FileName}",
            $"{WorkSpace}/{codeFile.FileName}",
            codeFile.FileName,
            FileKinds.Component
        );

        var razorCodeDocument = _projectEngine.Process(file);
        var cSharpDocument = razorCodeDocument.GetCSharpDocument();

        logs.Add(new CompileLog(Content: "Get GeneratedCode..."));
        var csCode = cSharpDocument.GeneratedCode;

        logs.Add(new CompileLog(Content: $"end compile razor {codeFile.FileName}"));
        logs.Add(new CompileLog(Content: $"razor csharp code: {Environment.NewLine} {csCode}"));
        return (csCode, cSharpDocument.Diagnostics);
    }

    private bool DiagnosticHasError(IEnumerable<RazorDiagnostic> diagnostics, List<CompileLog> logs)
    {
        logs.Add(new CompileLog(Content: "Read Diagnostics..."));

        bool error = false;
        foreach (var diagnostic in diagnostics)
        {
            LogLevel level = diagnostic.Severity == RazorDiagnosticSeverity.Warning
                ? LogLevel.Warning : LogLevel.Error;
            logs.Add(new CompileLog(Content: diagnostic.GetMessage(), level));
            if (diagnostic.Severity == RazorDiagnosticSeverity.Error)
            {
                error = true;
            }
        }

        if (error)
        {
            logs.Add(new CompileLog("compile end with error", LogLevel.Error));
        }
        return error;
    }

    private bool DiagnosticHasError(IEnumerable<Diagnostic> diagnostics, List<CompileLog> logs)
    {
        logs.Add(new CompileLog(Content: "Read Diagnostics..."));
        
        bool error = false;
        foreach (var diagnostic in diagnostics)
        {
            LogLevel level = diagnostic.Severity switch
            {
                DiagnosticSeverity.Info => LogLevel.Information,
                DiagnosticSeverity.Warning => LogLevel.Warning,
                DiagnosticSeverity.Error => LogLevel.Error,
                _ => LogLevel.Debug
            };
            
            logs.Add(new CompileLog(diagnostic.GetMessage(), level));
            if (diagnostic.Severity == DiagnosticSeverity.Error)
            {
                logs.Add(new CompileLog($"error source code:{Environment.NewLine}" + diagnostic.Location.SourceTree?.ToString(), level));
                error = true;
            }
        }

        if (error)
        {
            logs.Add(new CompileLog("compile end with error", LogLevel.Error));
        }
        return error;
    }

}