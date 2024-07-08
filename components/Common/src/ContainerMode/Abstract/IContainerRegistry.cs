
namespace AntDesign.Extensions;

public interface IContainerRegistry
{
    void AddOrUpdateOptions(string name, FragmentOptions obj);
    void RemoveOptions(string name, FragmentOptions obj);
    IDisposable Subscribe(string name, Action<FragmentsChangeArgs> action);
}