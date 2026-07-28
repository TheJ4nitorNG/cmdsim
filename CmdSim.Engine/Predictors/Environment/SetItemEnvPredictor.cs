using System.Linq;
using System.Management.Automation.Language;
using CmdSim.Sdk.Interfaces;
using CmdSim.Sdk.Models;
using CmdSim.Engine.Parsing;

namespace CmdSim.Engine.Predictors.Environment;

public class SetItemEnvPredictor : ICommandPredictor
{
    private readonly CommandParser _parser = new CommandParser();

    public bool Supports(string commandName)
    {
        return commandName.Equals("Set-Item", System.StringComparison.OrdinalIgnoreCase);
    }

    public SimulationResult Simulate(SimulationContext context, ParsedCommand parsedCommand)
    {
        var target = parsedCommand.Parameters.TryGetValue("Path", out var pt) ? pt : parsedCommand.Target;
        if (string.IsNullOrEmpty(target) || !target.StartsWith("Env:", System.StringComparison.OrdinalIgnoreCase))
        {
            // If it's not targeting Env:, this predictor doesn't actually handle it, 
            // but we intercepted it because the CommandName matched. Return safe.
            return new SimulationResult { Confidence = 100, Risk = RiskLevel.Safe, UndoPossible = true };
        }

        var path = target;
        var varName = path.Substring(4).TrimStart('\\', '/');

        var result = new SimulationResult
        {
            UndoPossible = true,
            Confidence = varName.Contains("$") ? 60 : 95,
            Risk = RiskLevel.Low
        };

        result.Effects.Add(new Effect
        {
            Category = "Environment",
            Description = $"Set environment variable '{varName}'"
        });

        return result;
    }
}
