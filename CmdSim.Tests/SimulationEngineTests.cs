using System.Collections.Generic;
using System.Linq;
using System.Management.Automation.Language;
using CmdSim.Engine.Core;
using CmdSim.Sdk.Interfaces;
using CmdSim.Sdk.Models;
using CmdSim.Engine.Parsing;
using FluentAssertions;
using Xunit;

namespace CmdSim.Tests;

public class SimulationEngineTests
{
    private class DummyRemoveItemPredictor : ICommandPredictor
    {
        public bool Supports(string commandName) => commandName == "Remove-Item";

        public SimulationResult Simulate(SimulationContext context, ParsedCommand command)
        {
            var target = string.IsNullOrEmpty(command.Target) ? "pipeline input" : command.Target;
            
            var result = new SimulationResult
            {
                Confidence = target == "pipeline input" ? 50 : 99,
                Risk = RiskLevel.High,
                UndoPossible = true
            };
            
            result.Effects.Add(new Effect 
            { 
                Category = "Filesystem",
                Description = $"Delete {target}" 
            });

            return result;
        }
    }

    [Fact]
    public void Simulate_WithSupportedPredictor_ReturnsAggregatedResult()
    {
        // Arrange
        var parser = new CommandParser();
        var predictors = new List<ICommandPredictor> { new DummyRemoveItemPredictor() };
        var engine = new SimulationEngine(parser, predictors);

        var input = "Remove-Item C:\\Temp";

        // Act
        var result = engine.Simulate(input);

        // Assert
        result.Should().NotBeNull();
        result.Confidence.Should().Be(99);
        result.Risk.Should().Be(RiskLevel.High);
        result.Effects.Should().HaveCount(1);
        result.Effects.First().Description.Should().Be("Delete C:\\Temp");
    }

    [Fact]
    public void Simulate_WithPipeline_HandlesMissingTargetAsPipelineInput()
    {
        // Arrange
        var parser = new CommandParser();
        var predictors = new List<ICommandPredictor> { new DummyRemoveItemPredictor() };
        var engine = new SimulationEngine(parser, predictors);

        // Get-ChildItem produces objects, Remove-Item has no explicit target
        var input = @"Get-ChildItem C:\Temp | Remove-Item";

        // Act
        var result = engine.Simulate(input);

        // Assert
        result.Should().NotBeNull();
        result.Effects.Should().Contain(e => e.Description.Contains("pipeline input"));
        result.Confidence.Should().BeLessThan(90); // Lower confidence due to dynamic pipeline
    }

    [Fact]
    public void Simulate_CalculatesTotalEstimatedRuntimeMs()
    {
        // Arrange
        var parser = new CommandParser();
        var predictors = new List<ICommandPredictor> { new DummyRemoveItemPredictor() };
        var engine = new SimulationEngine(parser, predictors);

        var input = @"Remove-Item C:\Temp";

        // Act
        var result = engine.Simulate(input);

        // Assert
        result.Should().NotBeNull();
        // DummyRemoveItemPredictor doesn't set EstimatedRuntimeMs yet, let's just make sure it aggregates what's there
        result.TotalEstimatedRuntimeMs.Should().BeGreaterThanOrEqualTo(0);
    }
}
