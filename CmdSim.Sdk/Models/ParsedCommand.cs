using System.Collections.Generic;
using System.Management.Automation.Language;

namespace CmdSim.Sdk.Models;

public class ParsedCommand
{
    public string CommandName { get; set; } = string.Empty;
    public string Target { get; set; } = string.Empty;
    
    // We will store parameters as a dictionary of Name -> Value string.
    // If it's a switch parameter, the value will be empty or "True".
    public Dictionary<string, string> Parameters { get; set; } = new();

    public CommandAst Ast { get; set; } = null!;
}
