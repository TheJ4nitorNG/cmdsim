using System.Linq;
using System.Management.Automation.Language;
using CmdSim.Sdk.Models;
using CmdSim.Engine.Parsing;
using CmdSim.Engine.Predictors.Registry;
using CmdSim.Engine.Predictors.Services;
using CmdSim.Engine.Predictors.Environment;
using FluentAssertions;
using Xunit;

namespace CmdSim.Tests;

public class Track3PredictorsTests
{
    private ParsedCommand ParseCommand(string input)
    {
        var parser = new CommandParser();
        return parser.Parse(input).First();
    }

    [Fact]
    public void NewItemPropertyPredictor_GeneratesRegistryEffect()
    {
        var parsedCommand = ParseCommand(@"New-ItemProperty -Path HKCU:\Software\MyApp -Name 'Setting' -Value 1");
        var predictor = new NewItemPropertyPredictor();
        
        predictor.Supports(parsedCommand.CommandName).Should().BeTrue();
        
        var context = new SimulationContext();
        var result = predictor.Simulate(context, parsedCommand);

        result.Effects.Should().ContainSingle();
        result.Effects.First().Category.Should().Be("Registry");
        result.Effects.First().Description.Should().Contain("Create registry value");
        result.Effects.First().Description.Should().Contain("Setting");
        result.Effects.First().Description.Should().Contain(@"HKCU:\Software\MyApp");
    }

    [Fact]
    public void StopServicePredictor_GeneratesServiceEffect()
    {
        var parsedCommand = ParseCommand(@"Stop-Service -Name wuauserv");
        var predictor = new StopServicePredictor();
        
        predictor.Supports(parsedCommand.CommandName).Should().BeTrue();
        
        var context = new SimulationContext();
        var result = predictor.Simulate(context, parsedCommand);

        result.Effects.Should().ContainSingle();
        result.Effects.First().Category.Should().Be("Service");
        result.Effects.First().Description.Should().Contain("Stop service");
        result.Effects.First().Description.Should().Contain("wuauserv");
    }

    [Fact]
    public void EnvItemPredictor_GeneratesEnvironmentEffect()
    {
        var parsedCommand = ParseCommand(@"Set-Item -Path Env:\MyVar -Value '123'");
        var predictor = new SetItemEnvPredictor();
        
        predictor.Supports(parsedCommand.CommandName).Should().BeTrue();
        
        var context = new SimulationContext();
        var result = predictor.Simulate(context, parsedCommand);

        result.Effects.Should().ContainSingle();
        result.Effects.First().Category.Should().Be("Environment");
        result.Effects.First().Description.Should().Contain("Set environment variable");
        result.Effects.First().Description.Should().Contain("MyVar");
    }
}
