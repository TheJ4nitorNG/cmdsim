using CmdSim.Sdk.Models;

namespace CmdSim.Engine.AI;

public interface IAiProvider
{
    string ExplainRisk(SimulationResult result);
}
