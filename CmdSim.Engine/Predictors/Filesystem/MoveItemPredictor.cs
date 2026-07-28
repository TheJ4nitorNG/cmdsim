using CmdSim.Sdk.Models;

namespace CmdSim.Engine.Predictors.Filesystem;

public class MoveItemPredictor : FilesystemPredictorBase
{
    public override string[] SupportedCommands => new[] { "Move-Item", "mi", "mv", "move" };

    protected override void GenerateEffects(ParsedCommand command, SimulationResult result)
    {
        string dest = command.Parameters.TryGetValue("Destination", out string? d) ? d : "Unknown Destination";

        result.Effects.Add(new Effect
        {
            Category = "Filesystem",
            Description = $"Move '{command.Target}' to '{dest}'"
        });
    }
}
