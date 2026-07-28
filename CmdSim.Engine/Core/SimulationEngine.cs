using System.Collections.Generic;
using System.Linq;
using CmdSim.Sdk.Interfaces;
using CmdSim.Sdk.Models;
using CmdSim.Engine.Parsing;
using CmdSim.Engine.State;

namespace CmdSim.Engine.Core;

public class SimulationEngine
{
    private readonly CommandParser _parser;
    private readonly IEnumerable<ICommandPredictor> _predictors;
    private readonly SimulationCache _cache;
    private readonly SessionHistoryTracker _historyTracker;

    public SimulationEngine(
        CommandParser parser, 
        IEnumerable<ICommandPredictor> predictors,
        SimulationCache? cache = null,
        SessionHistoryTracker? historyTracker = null)
    {
        _parser = parser;
        _predictors = predictors;
        _cache = cache ?? new SimulationCache();
        _historyTracker = historyTracker ?? new SessionHistoryTracker();
    }

    public SimulationResult Simulate(string input)
    {
        if (_cache.TryGet(input, out var cachedResult))
        {
            _historyTracker.Record(input, cachedResult);
            return cachedResult;
        }

        var context = new SimulationContext
        {
            CurrentDirectory = System.Environment.CurrentDirectory
        };

        var parsedCommands = _parser.Parse(input).ToList();
        
        var finalResult = new SimulationResult { Confidence = 100, Risk = RiskLevel.Safe, UndoPossible = true };

        foreach (var parsedCmd in parsedCommands)
        {
            var predictor = _predictors.FirstOrDefault(p => p.Supports(parsedCmd.CommandName));
            if (predictor != null)
            {
                var result = predictor.Simulate(context, parsedCmd);
                
                finalResult.Effects.AddRange(result.Effects);
                if (result.Risk > finalResult.Risk) finalResult.Risk = result.Risk;
                if (result.Confidence < finalResult.Confidence) finalResult.Confidence = result.Confidence;
                if (!result.UndoPossible) finalResult.UndoPossible = false;
            }
            else
            {
                // Unknown command in pipeline shouldn't necessarily fail the whole thing, but we lower confidence
                finalResult.Confidence = 0;
                finalResult.Risk = RiskLevel.Unknown;
            }
        }

        _cache.Add(input, finalResult);
        _historyTracker.Record(input, finalResult);

        return finalResult;
    }
}
