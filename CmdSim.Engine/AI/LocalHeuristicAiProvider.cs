using System.Linq;
using CmdSim.Sdk.Models;

namespace CmdSim.Engine.AI;

public class LocalHeuristicAiProvider : IAiProvider
{
    public string ExplainRisk(SimulationResult result)
    {
        if (result.Effects.Any(e => e.Category == "Filesystem" && e.Description.Contains("System32")))
        {
            return "This command modifies core Windows system files. Doing so can result in an unbootable system state or application corruption.";
        }

        if (result.Effects.Any(e => e.Category == "Registry" && e.Description.Contains("HKLM")))
        {
            return "This command modifies machine-wide registry keys, potentially affecting all users and system services.";
        }

        if (result.Effects.Any(e => e.Category == "Service" && e.Description.Contains("Stop")))
        {
            return "Stopping a service may cause dependent applications or system features to fail immediately.";
        }

        return "This command performs a high-risk operation. Proceed with extreme caution.";
    }
}
