using System.Xml.Linq;

namespace AntDesign.Extensions;

public abstract class ContainerServiceBase
{
    protected readonly IContainerRegistry _containerDispatcher;

    public ContainerServiceBase(IContainerRegistry containerDispatcher)
    {
        _containerDispatcher = containerDispatcher;
    }

    protected abstract string GetName();

    public virtual void AddOrUpdateOptions(FragmentOptions dialogOptions)
    {
        _containerDispatcher.AddOrUpdateOptions(GetName(), dialogOptions);
    }

    public virtual void RemoveOptions(FragmentOptions dialogOptions)
    {
        _containerDispatcher.RemoveOptions(GetName(), dialogOptions);
    }
}