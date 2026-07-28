namespace CmdSim.Sdk.Models;

public class Effect
{
    public string Category { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int EstimatedRuntimeMs { get; set; } = 0;
    
    public string? BeforeState { get; set; }
    public string? AfterState { get; set; }
}
