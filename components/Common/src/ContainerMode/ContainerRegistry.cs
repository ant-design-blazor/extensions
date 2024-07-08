using Microsoft.AspNetCore.Components;

namespace AntDesign.Extensions;


public sealed class ContainerRegistry : IContainerRegistry
{
    private Dictionary<string, HashSet<FragmentNode>> _fragments = new();
    private Dictionary<string, Action<FragmentsChangeArgs>> _subscriptions = new();

    public void AddOrUpdateOptions(string name, FragmentOptions obj)
    {
        foreach (var fragmentKv in _fragments)
        {
            var v = fragmentKv.Value;
        }
        if (!_fragments.ContainsKey(name))
        {
            _fragments[name] = new HashSet<FragmentNode>();
        }

        var fragmentModeInfo = new FragmentNode(name, obj);
        _fragments[name].Add(fragmentModeInfo);


        Dispatch(name);
    }

    public void RemoveOptions(string name, FragmentOptions obj)
    {
        if (!_fragments.ContainsKey(name))
        {
            return;
        }
        _fragments[name].RemoveWhere(x => x.Options.Key == obj.Key);

        Dispatch(name);
    }

    private void Dispatch(string name)
    {
        if (_subscriptions.TryGetValue(name, out var action))
        {
            var fragments = _fragments[name].Select(x => x.Options);
            var e = new FragmentsChangeArgs(fragments);
            action(e);
        }
    }

    /// <summary>
    /// container sub
    /// </summary>
    /// <param name="name"></param>
    /// <param name="action"></param>
    /// <returns></returns>
    public IDisposable Subscribe(string name, Action<FragmentsChangeArgs> action)
    {
        if (!_subscriptions.ContainsKey(name))
        {
            _subscriptions[name] = action;
        }

        return new ContainerSubscription(name, _subscriptions);
    }

    private class ContainerSubscription : IDisposable
    {
        private readonly string _name;
        private readonly Dictionary<string, Action<FragmentsChangeArgs>> _subscriptions;

        public ContainerSubscription(string name, Dictionary<string, Action<FragmentsChangeArgs>> subscriptions)
        {
            _name = name;
            _subscriptions = subscriptions;
        }

        public void Dispose()
        {
            _subscriptions.Remove(_name);
        }
    }
}
