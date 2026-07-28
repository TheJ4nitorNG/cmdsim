using CmdSim.Sdk.Models;

namespace CmdSim.Engine.Predictors.Filesystem;

public class NewItemPredictor : FilesystemPredictorBase
{
    public override string[] SupportedCommands => new[] { "New-Item", "ni" };

    protected override void GenerateEffects(ParsedCommand command, SimulationResult result)
    {
        string itemType = command.Parameters.TryGetValue("ItemType", out string? type) ? type : "Item";

        result.Effects.Add(new Effect
        {
            Category = "Filesystem",
            Description = $"Create {itemType} at '{command.Target}'"
        });
    }
}
