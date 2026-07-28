using System.Collections.Generic;
using System.Linq;

namespace CmdSim.Sdk.Models;

public class SimulationResult
{
    public int Confidence { get; set; }
    public RiskLevel Risk { get; set; }
    public bool UndoPossible { get; set; }
    public List<Effect> Effects { get; set; } = new();

    public int TotalEstimatedRuntimeMs => Effects.Sum(e => e.EstimatedRuntimeMs);
    
    public string? AiExplanation { get; set; }

    public static SimulationResult CreateUnknown()
    {
        return new SimulationResult
        {
            Confidence = 0,
            Risk = RiskLevel.Unknown,
            UndoPossible = false
        };
    }
}
