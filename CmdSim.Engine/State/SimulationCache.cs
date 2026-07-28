using System.Collections.Concurrent;
using CmdSim.Sdk.Models;

namespace CmdSim.Engine.State;

public class SimulationCache
{
    private readonly ConcurrentDictionary<string, SimulationResult> _cache = new();

    public bool TryGet(string input, out SimulationResult result)
    {
        return _cache.TryGetValue(input, out result!);
    }

    public void Add(string input, SimulationResult result)
    {
        _cache[input] = result;
    }

    public void Clear()
    {
        _cache.Clear();
    }
}
