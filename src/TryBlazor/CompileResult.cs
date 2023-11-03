using Microsoft.AspNetCore.Components;
using System.Reflection;

namespace AntDesign.Extensions;

public class CompileStreamResult
{
    public CompileStreamResult(List<CompileLog>? compileLogs = null, MemoryStream? stream = null)
    {
        Stream = stream;
        if (compileLogs != null)
        {
            CompileLogs = compileLogs;
        }
    }

    public MemoryStream? Stream { get; internal set; } = null;

    public List<CompileLog> CompileLogs { get; } = new();

    /// <summary>
    /// try blazor component error
    /// </summary>
    public Exception? Error { get; internal set; }
}

public class CompileResult
{
    public CompileResult(List<CompileLog>? compileLogs = null, Assembly? assembly = null)
    {
        Assembly = assembly;
        if (compileLogs != null)
        {
            CompileLogs = compileLogs;
        }
    }

    public Assembly? Assembly { get; internal set; } = null;

    public List<CompileLog> CompileLogs { get; } = new();

    /// <summary>
    /// try blazor component error
    /// </summary>
    public Exception? Error { get; internal set; }
}

public static class CompileResultExt
{
    public static RenderFragment? GetBlazorComponent(this CompileResult result)
    {
        var type = result.GetBlazorType();
       if (type != null)
       {
           return builder =>
           {
               builder.OpenComponent(0, type);
               builder.CloseComponent();
           };
       }

       return null;
    }

    public static Type? GetBlazorType(this CompileResult result)
    {
        if (result.Assembly == null)
        {
            return null;
        }
        return result.Assembly.GetExportedTypes()
            .FirstOrDefault(i => i.IsSubclassOf(typeof(ComponentBase)));
    }

    public static List<Type> GetBlazorTypes(this CompileResult result)
    {
        if (result.Assembly == null)
        {
            return new List<Type>();
        }
        return result.Assembly.GetExportedTypes()
            .Where(i => i.IsSubclassOf(typeof(ComponentBase)))
            .ToList();
    }

    public static T? Run<T>(this CompileResult result, string methodName, object?[]? parameters)
    {
        if (result.Assembly == null)
        {
            return default(T);

        }

        var type = result.Assembly.GetExportedTypes().FirstOrDefault();
        if (type == null)
        {
            return default(T);
        }

        var methodInfo = type.GetMethod(methodName);
        if (methodInfo == null)
        {
            return default(T);
        }

        var instance = Activator.CreateInstance(type);
        if (instance == null)
        {
            return default(T);
        }
        return (T?)methodInfo.Invoke(instance, parameters);
    }
}

