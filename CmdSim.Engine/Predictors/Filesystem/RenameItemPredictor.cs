using System.Linq;
using CmdSim.Sdk.Models;

namespace CmdSim.Engine.Predictors.Filesystem;

public class RenameItemPredictor : FilesystemPredictorBase
{
    public override string[] SupportedCommands => new[] { "Rename-Item", "rni", "ren" };

    protected override void GenerateEffects(ParsedCommand command, SimulationResult result)
    {
        string newName = command.Parameters.TryGetValue("NewName", out string? n) ? n : "Unknown Name";

        // Heuristic: If NewName wasn't explicitly named, it might be the second positional parameter.
        if (newName == "Unknown Name")
        {
            if (command.Parameters.TryGetValue("Positional_1", out string? pos1))
            {
                newName = pos1;
            }
        }

        result.Effects.Add(new Effect
        {
            Category = "Filesystem",
            Description = $"Rename '{command.Target}' to '{newName}'",
            BeforeState = command.Target,
            AfterState = newName,
            EstimatedRuntimeMs = 25
        });
    }
}
