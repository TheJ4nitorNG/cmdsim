using System.Linq;
using CmdSim.Sdk.Models;
using CmdSim.Engine.Parsing;
using FluentAssertions;
using Xunit;

namespace CmdSim.Tests;

public class CommandParserTests
{
    [Fact]
    public void Parse_RemoveItem_ExtractsCommandParametersAndTarget()
    {
        // Arrange
        var parser = new CommandParser();
        var input = @"Remove-Item C:\Logs -Recurse";

        // Act
        var result = parser.Parse(input).ToList();

        // Assert
        result.Should().HaveCount(1);
        var command = result.First();
        
        command.CommandName.Should().Be("Remove-Item");
        command.Target.Should().Be(@"C:\Logs");
        command.Parameters.Should().ContainKey("Recurse");
        
        // Note: 'Path' might be inferred as positional, but explicitly it's a positional argument.
        // We will represent positional arguments as parameters with empty names or mapped to target.
        // Let's ensure Target represents the primary positional argument or pipeline input.
    }

    [Fact]
    public void Parse_MoveItem_ExtractsCommandParametersAndTarget()
    {
        // Arrange
        var parser = new CommandParser();
        var input = @"Move-Item -Path C:\Logs -Destination D:\Logs -Force";

        // Act
        var result = parser.Parse(input).ToList();

        // Assert
        result.Should().HaveCount(1);
        var command = result.First();
        
        command.CommandName.Should().Be("Move-Item");
        // Target can be the Path parameter
        command.Target.Should().Be(@"C:\Logs");
        command.Parameters.Should().ContainKey("Destination").WhoseValue.Should().Be(@"D:\Logs");
        command.Parameters.Should().ContainKey("Force");
    }
}
