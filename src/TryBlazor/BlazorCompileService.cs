using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Net.Http.Json;
using System.Reflection;
using System.Runtime;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Routing;
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
        var tasks = new List<Task>();
        var loadedAssembly = new HashSet<string>();
        foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
        {
            if (assembly.IsDynamic)
            {
                continue;
            }

            var name = assembly.GetName().Name;
            if (string.IsNullOrWhiteSpace(name))
            {
                continue;
            }
            loadedAssembly.Add(name);
            tasks.Add(LoadReference(name + ".dll"));
        }

        var assemblyNames = _blazorCompileServiceOptions.AdditionalAssemblys
            .SelectMany(assembly => assembly.GetReferencedAssemblies().Concat(new[] { assembly.GetName() }))
            .Select(x => x.Name)
            .Distinct()
            .ToList();

        foreach (var name in assemblyNames)
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

        var basicReferenceAssemblyRoots = new[]
        {
            typeof(Console).Assembly, // System.Console
            typeof(Uri).Assembly, // System.Private.Uri
            typeof(AssemblyTargetedPatchBandAttribute).Assembly, // System.Private.CoreLib
            typeof(NavLink).Assembly, // Microsoft.AspNetCore.Components.Web
            typeof(IQueryable).Assembly, // System.Linq.Expressions
            typeof(HttpClientJsonExtensions).Assembly, // System.Net.Http.Json
            typeof(HttpClient).Assembly, // System.Net.Http
            typeof(IJSRuntime).Assembly, // Microsoft.JSInterop
            typeof(RequiredAttribute).Assembly, // System.ComponentModel.Annotations
        };
        var b = basicReferenceAssemblyRoots.SelectMany(assembly => assembly.GetReferencedAssemblies().Concat(new[] { assembly.GetName() }))
            .Select(x => x.Name)
            .Distinct()
            .ToList();
        foreach (var name in b)
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
    }

    public async Task LoadReference(string name)
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
            _references.Add(MetadataReference.CreateFromStream(stream));
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

    public async Task<CompileResult> Compile(
        List<CodeInfo> codeInfos,
        string assemblyName = nameof(BlazorCompileService),
        OptimizationLevel optimizationLevel = OptimizationLevel.Release
        )
    {
        if (codeInfos == null || codeInfos.Count == 0)
        {
            throw new ArgumentException(nameof(codeInfos) + "cannot be null or empty");
        }

        var logs = new List<CompileLog>();
        var result = new CompileResult(logs);
        await (_initTask ??= SafeInitEngine());
        Debug.Assert(_projectEngine != null);

        logs.Add(new CompileLog(Content: "begin compile..."));

        var syntaxTrees = new List<SyntaxTree>();
        foreach (CodeInfo codeInfo in codeInfos)
        {
            var csharpCode = codeInfo.Code;
            if (codeInfo.CodeType == CodeType.Razor)
            {
                var (code, diagnostics) = CompileRazorToCSharp(codeInfo, logs);
                if (DiagnosticHasError(diagnostics, logs))
                {
                    return result;
                }
                csharpCode = code;
            }
            // else must be c#, Otherwise, an exception will be thrown (codeInfo.CodeType's getter)

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
                        new KeyValuePair<string, ReportDiagnostic>("CS1702", ReportDiagnostic.Suppress),
                    }
                )
            );
        var (assembly, _) = CompileToAssembly(
            compilation,
            syntaxTrees,
            _references,
            logs
        );

        result.Assembly = assembly;
        logs.Add(new CompileLog(Content: $"end compile {assemblyName}, result.Assembly is null:{result.Assembly == null}"));
        return result;

        #region two phase compilation
        ////https://github.com/dotnet/razor/blob/6a5ccd98416f265b5c6ab8d06786e1ed19861363/src/Shared/Microsoft.AspNetCore.Razor.Test.Common/Language/IntegrationTests/RazorIntegrationTestBase.cs#L199 
        //var baseCompilation = CSharpCompilation.Create(
        //    _blazorCompileServiceOptions.RootNamespace,
        //    Array.Empty<SyntaxTree>(),
        //    _references,
        //    new CSharpCompilationOptions(
        //        OutputKind.DynamicallyLinkedLibrary,
        //        optimizationLevel: OptimizationLevel.Release,
        //        concurrentBuild: false,
        //        specificDiagnosticOptions: new[]
        //        {
        //            new KeyValuePair<string, ReportDiagnostic>("CS1701", ReportDiagnostic.Suppress),
        //            new KeyValuePair<string, ReportDiagnostic>("CS1702", ReportDiagnostic.Suppress),
        //        }));

        //var cSharpParseOptions = new CSharpParseOptions(LanguageVersion.Preview);
        //List<SyntaxTree> AdditionalSyntaxTrees = new List<SyntaxTree>();

        //// 第一阶段
        //var projectEngine = this.CreateRazorProjectEngine(Array.Empty<MetadataReference>());
        //RazorCodeDocument codeDocument = projectEngine.ProcessDeclarationOnly(file);

        //var syntaxTree = (CSharpSyntaxTree)CSharpSyntaxTree.ParseText(
        //    codeDocument.GetCSharpDocument().GeneratedCode,
        //    cSharpParseOptions,
        //    file.FilePath);
        //AdditionalSyntaxTrees.Add(syntaxTree);

        //baseCompilation = baseCompilation.AddSyntaxTrees(syntaxTree);
        //var references = new List<MetadataReference>(_references)
        //{
        //    baseCompilation.ToMetadataReference()
        //};


        //// 第二阶段
        //var engine2 = this.CreateRazorProjectEngine(references);
        //var docs2 = engine2.Process(file);
        //var t2 = CSharpSyntaxTree.ParseText(
        //    docs2.GetCSharpDocument().GeneratedCode, 
        //    cSharpParseOptions, 
        //    file.FilePath
        //    );
        //baseCompilation = baseCompilation.AddSyntaxTrees(t2);


        //using var peStream = new MemoryStream();
        //baseCompilation.Emit(peStream);
        //peStream.Seek(0, SeekOrigin.Begin);

        //Assembly assembly1 = AppDomain.CurrentDomain.Load(peStream.ToArray());
        //result.Assembly = assembly1; 
        #endregion
    }

    private (Assembly?, CSharpCompilation) CompileToAssembly(
        CSharpCompilation baseCompilation,
        List<SyntaxTree> syntaxTrees,
        List<MetadataReference> references,
        List<CompileLog> logs)
    {
        var compilation = baseCompilation.AddSyntaxTrees(syntaxTrees)
            .AddReferences(_references);

        using var stream = new MemoryStream();
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
        var assembly = AppDomain.CurrentDomain.Load(stream.ToArray());
        return (assembly, compilation);
    }

    private bool DiagnosticHasError(IEnumerable<RazorDiagnostic> diagnostics, List<CompileLog> logs)
    {
        logs.Add(new CompileLog(Content: "Read Diagnostics..."));

        bool error = false;
        foreach (var diagnostic in diagnostics)
        {
            LogLevel level = diagnostic.Severity == RazorDiagnosticSeverity.Warning
                ? LogLevel.Warning : LogLevel.Error;
            logs.Add(new CompileLog(Content: diagnostic.ToString(), level));
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
            logs.Add(new CompileLog(diagnostic.ToString(), level));
            if (diagnostic.Severity == DiagnosticSeverity.Error)
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

    private (string, IReadOnlyList<RazorDiagnostic>) CompileRazorToCSharp(CodeInfo codeInfo, List<CompileLog> logs)
    {
        Debug.Assert(_projectEngine != null);

        logs.Add(new CompileLog(Content: $"begin compile razor {codeInfo.FileName}"));

        var file = new VirtualProjectItem(
            codeInfo.Code,
            WorkSpace,
            $"{WorkSpace}/{codeInfo.FileName}",
            $"{WorkSpace}/{codeInfo.FileName}",
            codeInfo.FileName,
            FileKinds.Component
        );

        var razorCodeDocument = _projectEngine.Process(file);
        var cSharpDocument = razorCodeDocument.GetCSharpDocument();

        logs.Add(new CompileLog(Content: "Get GeneratedCode..."));
        var csCode = cSharpDocument.GeneratedCode;

        logs.Add(new CompileLog(Content: $"end compile razor {codeInfo.FileName}"));
        logs.Add(new CompileLog(Content: $"razor csharp code: {Environment.NewLine} {csCode}"));
        return (csCode, cSharpDocument.Diagnostics);
    }
}