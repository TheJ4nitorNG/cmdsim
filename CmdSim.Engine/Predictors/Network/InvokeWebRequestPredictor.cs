using System.Linq;
using CmdSim.Sdk.Interfaces;
using CmdSim.Sdk.Models;

namespace CmdSim.Engine.Predictors.Network;

public class InvokeWebRequestPredictor : ICommandPredictor
{
    public bool Supports(string commandName)
    {
        return commandName.Equals("Invoke-WebRequest", System.StringComparison.OrdinalIgnoreCase) ||
               commandName.Equals("iwr", System.StringComparison.OrdinalIgnoreCase);
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
            Risk = RiskLevel.Low // Data egress is generally low risk unless it's authenticated/internal, but we start at Low
        };

        result.Effects.Add(new Effect
        {
            Category = "Network",
            Description = $"HTTP {method} request to '{uri}'",
            EstimatedRuntimeMs = 300 // heuristic for average web request latency
        });

        return result;
    }
}
