using System.Linq;
using System.Management.Automation.Language;
using CmdSim.Sdk.Interfaces;
using CmdSim.Sdk.Models;
using CmdSim.Engine.Parsing;

namespace CmdSim.Engine.Predictors.Services;

public class StopServicePredictor : ICommandPredictor
{
    private readonly CommandParser _parser = new CommandParser();

    public bool Supports(string commandName)
    {
        return commandName.Equals("Stop-Service", System.StringComparison.OrdinalIgnoreCase);
    }

    public SimulationResult Simulate(SimulationContext context, ParsedCommand parsedCommand)
    {
        var name = parsedCommand.Parameters.TryGetValue("Name", out var n) ? n : parsedCommand.Target;

        var result = new SimulationResult
        {
            UndoPossible = false, 
            Confidence = name.Contains("$") ? 60 : 98,
            Risk = RiskLevel.High 
        };

        result.Effects.Add(new Effect
        {
            Category = "Service",
            Description = $"Stop service '{name}'"
        });

        return result;
    }
}
