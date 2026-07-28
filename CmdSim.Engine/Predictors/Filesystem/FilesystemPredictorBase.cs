using System;
using System.Linq;
using System.Management.Automation.Language;
using CmdSim.Sdk.Interfaces;
using CmdSim.Sdk.Models;
using CmdSim.Engine.Parsing;

namespace CmdSim.Engine.Predictors.Filesystem;

public abstract class FilesystemPredictorBase : ICommandPredictor
{
    private readonly CommandParser _parser = new CommandParser();

    public abstract string[] SupportedCommands { get; }

    public bool Supports(string commandName)
    {
        return SupportedCommands.Contains(commandName, StringComparer.OrdinalIgnoreCase);
    }

    public SimulationResult Simulate(SimulationContext context, ParsedCommand parsedCommand)
    {
        var result = new SimulationResult
        {
            UndoPossible = true,
            Confidence = CalculateConfidence(parsedCommand),
            Risk = CalculateRisk(parsedCommand)
        };

        GenerateEffects(parsedCommand, result);

        return result;
    }

    protected abstract void GenerateEffects(ParsedCommand command, SimulationResult result);

    protected virtual int CalculateConfidence(ParsedCommand command)
    {
        if (command.Target.Contains("$") || command.Parameters.Values.Any(v => v.Contains("$")))
        {
            return 45;
        }
        return 95;
    }

    protected virtual RiskLevel CalculateRisk(ParsedCommand command)
    {
        string target = command.Target.ToLowerInvariant();
        if (target.Contains(@"c:\windows") || target.Contains("system32"))
        {
            return RiskLevel.Critical;
        }
        if (target.Contains(@"c:\program files"))
        {
            return RiskLevel.High;
        }
        if (target == @"\" || target == @"c:\")
        {
            return RiskLevel.Critical;
        }
        return RiskLevel.Low;
    }
}
