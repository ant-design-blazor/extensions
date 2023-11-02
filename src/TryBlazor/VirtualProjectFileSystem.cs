using Microsoft.AspNetCore.Razor.Language;

namespace AntDesign.Extensions;

internal partial class VirtualRazorProjectFileSystem : RazorProjectFileSystem
{
    public override IEnumerable<RazorProjectItem> EnumerateItems(string basePath)
    {
        this.NormalizeAndEnsureValidPath(basePath);
        return Enumerable.Empty<RazorProjectItem>();
    }

    public override RazorProjectItem GetItem(string path) => this.GetItem(path, fileKind: null);

    public override RazorProjectItem GetItem(string path, string fileKind)
    {
        this.NormalizeAndEnsureValidPath(path);
        return new NotFoundProjectItem(string.Empty, path);
    }
}