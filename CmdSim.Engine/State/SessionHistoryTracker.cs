using System;
using System.Collections.Generic;
using CmdSim.Sdk.Models;

namespace CmdSim.Engine.State;

public class SessionHistoryEntry
{
    public DateTime Timestamp { get; set; }
    public string Command { get; set; } = string.Empty;
    public SimulationResult Result { get; set; } = null!;
}

public class SessionHistoryTracker
{
    private readonly List<SessionHistoryEntry> _history = new();

    public IReadOnlyList<SessionHistoryEntry> GetHistory() => _history.AsReadOnly();

    public void Record(string command, SimulationResult result)
    {
        _history.Add(new SessionHistoryEntry
        {
            Timestamp = DateTime.UtcNow,
            Command = command,
            Result = result
        });
    }
}
