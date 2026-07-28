using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation.Language;
using CmdSim.Sdk.Models;

namespace CmdSim.Engine.Parsing;

public class CommandParser
{
    public IEnumerable<ParsedCommand> Parse(string input)
    {
        var ast = Parser.ParseInput(input, out Token[] tokens, out ParseError[] errors);

        if (errors.Any())
        {
            throw new ArgumentException($"Failed to parse input: {string.Join(", ", errors.Select(e => e.Message))}");
        }

        var pipelineAsts = ast.FindAll(node => node is PipelineAst, searchNestedScriptBlocks: true).Cast<PipelineAst>();

        foreach (var pipeline in pipelineAsts)
        {
            var elements = pipeline.PipelineElements;
            for (int i = 0; i < elements.Count; i++)
            {
                if (elements[i] is CommandAst cmdAst)
                {
                    var parsedCommand = new ParsedCommand
                    {
                        CommandName = cmdAst.GetCommandName(),
                        Ast = cmdAst
                    };

                    var cmdElements = cmdAst.CommandElements.ToList();
                    
                    for (int j = 1; j < cmdElements.Count; j++) // Start at 1 to skip command name
                    {
                        var element = cmdElements[j];

                        if (element is CommandParameterAst paramAst)
                        {
                            string paramName = paramAst.ParameterName;
                            
                            if (paramAst.Argument != null)
                            {
                                parsedCommand.Parameters[paramName] = paramAst.Argument.Extent.Text;
                            }
                            else if (j + 1 < cmdElements.Count && !(cmdElements[j + 1] is CommandParameterAst))
                            {
                                parsedCommand.Parameters[paramName] = cmdElements[j + 1].Extent.Text;
                                j++;
                            }
                            else
                            {
                                parsedCommand.Parameters[paramName] = "True"; 
                            }
                        }
                        else
                        {
                            if (string.IsNullOrEmpty(parsedCommand.Target))
                            {
                                parsedCommand.Target = element.Extent.Text;
                            }
                            else
                            {
                                int positionalCount = parsedCommand.Parameters.Keys.Count(k => k.StartsWith("Positional_"));
                                parsedCommand.Parameters[$"Positional_{positionalCount + 1}"] = element.Extent.Text;
                            }
                        }
                    }

                    if (string.IsNullOrEmpty(parsedCommand.Target))
                    {
                        if (parsedCommand.Parameters.TryGetValue("Path", out string? pathTarget))
                            parsedCommand.Target = pathTarget;
                        else if (parsedCommand.Parameters.TryGetValue("Name", out string? nameTarget))
                            parsedCommand.Target = nameTarget;
                        else if (parsedCommand.Parameters.TryGetValue("Identity", out string? identityTarget))
                            parsedCommand.Target = identityTarget;
                    }

                    // If part of a pipeline (not the first element) and Target is still empty
                    if (i > 0 && string.IsNullOrEmpty(parsedCommand.Target))
                    {
                        parsedCommand.Target = "pipeline input";
                    }

                    yield return parsedCommand;
                }
            }
        }
    }
}
