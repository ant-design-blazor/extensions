namespace AntDesign.Extensions;

internal class FragmentNodeHelper
{
    private static int _key = 0;
    public static int GenKey()
    {
        return Interlocked.Increment(ref _key);
    }
}
