using System.Linq;
using System.Management.Automation.Language;
using CmdSim.Sdk.Interfaces;
using CmdSim.Sdk.Models;
using CmdSim.Engine.Parsing;

namespace CmdSim.Engine.Predictors.Registry;

public class NewItemPropertyPredictor : ICommandPredictor
{
    private readonly CommandParser _parser = new CommandParser();

    public bool Supports(string commandName)
    {
        return commandName.Equals("New-ItemProperty", System.StringComparison.OrdinalIgnoreCase);
    }

    public SimulationResult Simulate(SimulationContext context, ParsedCommand parsedCommand)
    {
        var path = parsedCommand.Parameters.TryGetValue("Path", out var p) ? p : parsedCommand.Target;
        var name = parsedCommand.Parameters.TryGetValue("Name", out var n) ? n : "Unknown";

        var result = new SimulationResult
        {
            UndoPossible = true,
            Confidence = path.Contains("$") || name.Contains("$") ? 50 : 95,
            Risk = path.ToLowerInvariant().Contains(@"hklm:\system") || path.ToLowerInvariant().Contains(@"hklm:\software\microsoft\windows") ? RiskLevel.High : RiskLevel.Medium
        };

        result.Effects.Add(new Effect
        {
            Category = "Registry",
            Description = $"Create registry value '{name}' in '{path}'"
        });

        return result;
    }
}
