using System.Text;
using Microsoft.AspNetCore.Razor.Language;

namespace AntDesign.Extensions;

internal class VirtualProjectItem : RazorProjectItem
{
    private readonly byte[] content;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="code"></param>
    /// <param name="basePath"></param>
    /// <param name="filePath"></param>
    /// <param name="physicalPath"></param>
    /// <param name="relativePhysicalPath"></param>
    /// <param name="fileKind"> FileKinds.Component </param>
    public VirtualProjectItem(
        string code,
        string basePath = "/App",
        string filePath = "/App/App.razor",
        string physicalPath = "/App/App.razor",
        string relativePhysicalPath = "App.razor",
        string fileKind = "component"
       )
    {
        this.BasePath = basePath;
        this.FilePath = filePath;
        this.PhysicalPath = physicalPath;
        this.RelativePhysicalPath = relativePhysicalPath;
        this.content = Encoding.UTF8.GetBytes(code);

        // Base class will detect based on file-extension.
        this.FileKind = fileKind ?? base.FileKind;
    }

    public override string BasePath { get; }

    public override string RelativePhysicalPath { get; }

    public override string FileKind { get; }

    public override string FilePath { get; }

    public override string PhysicalPath { get; }

    public override bool Exists => true;

    public string FileContent => Encoding.UTF8.GetString(this.content);

    public override Stream Read() => new MemoryStream(this.content);
}