using System.Diagnostics.CodeAnalysis;

namespace AntDesign.Extensions;

[ExcludeFromCodeCoverage]
public class FragmentOptions
{
    public int Key { get; } = FragmentNodeHelper.GenKey();
}
