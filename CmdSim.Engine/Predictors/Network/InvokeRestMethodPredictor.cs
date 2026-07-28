using System.Linq;
using CmdSim.Sdk.Interfaces;
using CmdSim.Sdk.Models;

namespace CmdSim.Engine.Predictors.Network;

public class InvokeRestMethodPredictor : ICommandPredictor
{
    public bool Supports(string commandName)
    {
        return commandName.Equals("Invoke-RestMethod", System.StringComparison.OrdinalIgnoreCase) ||
               commandName.Equals("irm", System.StringComparison.OrdinalIgnoreCase);
    }

    public SimulationResult Simulate(SimulationContext context, ParsedCommand parsedCommand)
    {
        var uri = parsedCommand.Parameters.TryGetValue("Uri", out var u) ? u : parsedCommand.Target;
        var method = parsedCommand.Parameters.TryGetValue("Method", out var m) ? m : "GET";

        // Trim surrounding quotes if present from raw text
        uri = uri.Trim('\'', '\"');
        method = method.Trim('\'', '\"');

        var result = new SimulationResult
        {
            UndoPossible = false, 
            Confidence = uri.Contains("$") ? 40 : 95,
            Risk = RiskLevel.Low
        };

        result.Effects.Add(new Effect
        {
            Category = "Network",
            Description = $"HTTP {method} request to '{uri}'"
        });

        return result;
    }
}
