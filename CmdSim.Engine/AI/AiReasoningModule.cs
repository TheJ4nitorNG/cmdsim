using CmdSim.Sdk.Models;

namespace CmdSim.Engine.AI;

public class AiReasoningModule
{
    private readonly IAiProvider _provider;

    public AiReasoningModule(IAiProvider provider)
    {
        _provider = provider;
    }

    public void Analyze(SimulationResult result)
    {
        if (result.Risk >= RiskLevel.High)
        {
            result.AiExplanation = _provider.ExplainRisk(result);
        }
    }
}
