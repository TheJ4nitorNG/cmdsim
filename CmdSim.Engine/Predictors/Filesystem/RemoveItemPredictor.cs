using CmdSim.Sdk.Models;

namespace CmdSim.Engine.Predictors.Filesystem;

public class RemoveItemPredictor : FilesystemPredictorBase
{
    public override string[] SupportedCommands => new[] { "Remove-Item", "ri", "rm", "del", "erase", "rd" };

    protected override void GenerateEffects(ParsedCommand command, SimulationResult result)
    {
        result.Effects.Add(new Effect
        {
            Category = "Filesystem",
            Description = $"Delete '{command.Target}'",
            EstimatedRuntimeMs = 15 // heuristic for single file delete
        });
    }
}
