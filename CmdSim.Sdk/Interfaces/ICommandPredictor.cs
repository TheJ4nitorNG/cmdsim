using CmdSim.Sdk.Models;

namespace CmdSim.Sdk.Interfaces;

public interface ICommandPredictor
{
    bool Supports(string commandName);
    SimulationResult Simulate(SimulationContext context, ParsedCommand command);
}
