using System.Collections.Generic;
using CmdSim.Engine.Core;
using CmdSim.Engine.Parsing;
using CmdSim.Engine.State;
using CmdSim.Sdk.Interfaces;
using FluentAssertions;
using Xunit;

namespace CmdSim.Tests;

public class EngineStateTests
{
    [Fact]
    public void Simulate_RepeatedCalls_ReturnsCachedResultAndUpdatesHistory()
    {
        var parser = new CommandParser();
        var predictors = new List<ICommandPredictor>(); // Empty is fine, it will return Unknown
        var cache = new SimulationCache();
        var history = new SessionHistoryTracker();
        
        var engine = new SimulationEngine(parser, predictors, cache, history);
        
        var input = "Get-ChildItem";
        
        // First call
        var result1 = engine.Simulate(input);
        
        // Second call
        var result2 = engine.Simulate(input);

        // Assert
        result1.Should().BeSameAs(result2); // Should be the exact same object reference from cache
        
        var recordedHistory = history.GetHistory();
        recordedHistory.Should().HaveCount(2);
        recordedHistory[0].Command.Should().Be("Get-ChildItem");
        recordedHistory[1].Command.Should().Be("Get-ChildItem");
    }
}
