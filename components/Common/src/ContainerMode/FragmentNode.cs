namespace AntDesign.Extensions;

public sealed record FragmentNode(string Name, FragmentOptions Options)
{
    public override int GetHashCode()
    {
        return Options.Key.GetHashCode();
    }
}
