using System.Linq;
using System.Management.Automation.Language;
using CmdSim.Sdk.Models;
using CmdSim.Engine.Parsing;
using CmdSim.Engine.Predictors.Network;
using FluentAssertions;
using Xunit;

namespace CmdSim.Tests;

public class NetworkPredictorsTests
{
    private ParsedCommand ParseCommand(string input)
    {
        var parser = new CommandParser();
        return parser.Parse(input).First();
    }

    [Fact]
    public void InvokeWebRequestPredictor_GeneratesNetworkEffect()
    {
        var parsedCommand = ParseCommand(@"Invoke-WebRequest -Uri 'https://api.github.com/users' -Method GET");
        var predictor = new InvokeWebRequestPredictor();
        
        predictor.Supports(parsedCommand.CommandName).Should().BeTrue();
        
        var context = new SimulationContext();
        var result = predictor.Simulate(context, parsedCommand);

        result.Effects.Should().ContainSingle();
        result.Effects.First().Category.Should().Be("Network");
        result.Effects.First().Description.Should().Contain("HTTP GET request");
        result.Effects.First().Description.Should().Contain("https://api.github.com/users");
    }

    [Fact]
    public void InvokeRestMethodPredictor_GeneratesNetworkEffect()
    {
        var parsedCommand = ParseCommand(@"Invoke-RestMethod -Uri 'https://api.example.com/data' -Method POST");
        var predictor = new InvokeRestMethodPredictor();
        
        predictor.Supports(parsedCommand.CommandName).Should().BeTrue();
        
        var context = new SimulationContext();
        var result = predictor.Simulate(context, parsedCommand);

        result.Effects.Should().ContainSingle();
        result.Effects.First().Category.Should().Be("Network");
        result.Effects.First().Description.Should().Contain("HTTP POST request");
        result.Effects.First().Description.Should().Contain("https://api.example.com/data");
    }
}
