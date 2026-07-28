using CmdSim.Sdk.Models;
using Moq;
using Xunit;
using FluentAssertions;
using CmdSim.Engine.AI;

namespace CmdSim.Tests;

public class AiReasoningTests
{
    private class MockAiProvider : IAiProvider
    {
        public string ExplainRisk(SimulationResult result)
        {
            return "This is a mock AI explanation.";
        }
    }

    [Fact]
    public void Analyze_HighRisk_PopulatesExplanation()
    {
        var result = new SimulationResult { Risk = RiskLevel.High };
        var module = new AiReasoningModule(new MockAiProvider());

        module.Analyze(result);

        result.AiExplanation.Should().Be("This is a mock AI explanation.");
    }

    [Fact]
    public void Analyze_LowRisk_DoesNotPopulateExplanation()
    {
        var result = new SimulationResult { Risk = RiskLevel.Low };
        var module = new AiReasoningModule(new MockAiProvider());

        module.Analyze(result);

        result.AiExplanation.Should().BeNull();
    }
}
