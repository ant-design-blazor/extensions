﻿using Microsoft.AspNetCore.Razor.Language;

namespace AntDesign.Extensions;

internal partial class NotFoundProjectItem : RazorProjectItem
{
    public NotFoundProjectItem(string basePath, string path)
    {
        this.BasePath = basePath;
        this.FilePath = path;
    }

    public override string BasePath { get; }

    public override string FilePath { get; }

    public override bool Exists => false;

    public override string PhysicalPath => throw new NotSupportedException();

    public override Stream Read() => throw new NotSupportedException();
}