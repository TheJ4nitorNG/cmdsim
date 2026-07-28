using System.Management.Automation.Language;
using CmdSim.Sdk.Models;
using CmdSim.Engine.Parsing;
using CmdSim.Engine.Predictors.Filesystem;
using FluentAssertions;
using Xunit;
using System.Linq;

namespace CmdSim.Tests;

public class FilesystemPredictorsTests
{
    private ParsedCommand ParseCommand(string input)
    {
        var parser = new CommandParser();
        return parser.Parse(input).First();
    }

    [Fact]
    public void RemoveItemPredictor_WithSafePath_ReturnsExpectedResult()
    {
        var parsedCommand = ParseCommand(@"Remove-Item C:\Temp\test.txt -Force");
        var predictor = new RemoveItemPredictor();
        
        predictor.Supports(parsedCommand.CommandName).Should().BeTrue();
        
        var context = new SimulationContext();
        var result = predictor.Simulate(context, parsedCommand);

        result.Confidence.Should().BeGreaterThan(80);
        result.Risk.Should().Be(RiskLevel.Low);
        result.UndoPossible.Should().BeTrue();
        result.Effects.Should().ContainSingle();
        result.Effects.First().Description.Should().Contain("Delete");
        result.Effects.First().Description.Should().Contain(@"C:\Temp\test.txt");
    }

    [Fact]
    public void RemoveItemPredictor_WithDangerousPath_ReturnsHighRisk()
    {
        var parsedCommand = ParseCommand(@"Remove-Item C:\Windows\System32\config.sys -Force");
        var predictor = new RemoveItemPredictor();
        var context = new SimulationContext();
        
        var result = predictor.Simulate(context, parsedCommand);

        result.Risk.Should().Be(RiskLevel.Critical);
    }

    [Fact]
    public void NewItemPredictor_WithVariable_ReturnsLowerConfidence()
    {
        var parsedCommand = ParseCommand(@"New-Item $targetPath -ItemType Directory");
        var predictor = new NewItemPredictor();
        var context = new SimulationContext();
        
        var result = predictor.Simulate(context, parsedCommand);

        result.Confidence.Should().BeLessThan(80);
        result.Effects.First().Description.Should().Contain("$targetPath");
    }

    [Fact]
    public void MoveItemPredictor_GeneratesMoveEffect()
    {
        var parsedCommand = ParseCommand(@"Move-Item -Path C:\source.txt -Destination D:\dest.txt");
        var predictor = new MoveItemPredictor();
        var context = new SimulationContext();
        
        var result = predictor.Simulate(context, parsedCommand);

        result.Effects.First().Description.Should().Contain("Move");
        result.Effects.First().Description.Should().Contain(@"C:\source.txt");
        result.Effects.First().Description.Should().Contain(@"D:\dest.txt");
    }

    [Fact]
    public void RenameItemPredictor_GeneratesRenameEffect()
    {
        var parsedCommand = ParseCommand(@"Rename-Item old.txt new.txt");
        var predictor = new RenameItemPredictor();
        var context = new SimulationContext();
        
        var result = predictor.Simulate(context, parsedCommand);

        result.Effects.First().Description.Should().Contain("Rename");
        result.Effects.First().Description.Should().Contain("old.txt");
        result.Effects.First().Description.Should().Contain("new.txt");
    }
}
