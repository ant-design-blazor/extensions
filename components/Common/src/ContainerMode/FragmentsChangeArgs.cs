namespace AntDesign.Extensions;

public sealed class FragmentsChangeArgs(IEnumerable<FragmentOptions> fragments)
{
    public IEnumerable<FragmentOptions> Fragments => fragments;
}
