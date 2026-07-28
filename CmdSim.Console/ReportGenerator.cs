using System.Linq;
using CmdSim.Sdk.Models;
using Spectre.Console;

namespace CmdSim.ConsoleApp;

public class ReportGenerator
{
    public void Render(SimulationResult result, string commandInput)
    {
        AnsiConsole.Write(new Rule("[yellow]Simulation Summary[/]").Centered());
        AnsiConsole.WriteLine();

        var grid = new Grid()
            .AddColumn(new GridColumn().PadRight(2).NoWrap())
            .AddColumn();

        grid.AddRow("[bold]Command[/]", $"[cyan]{Markup.Escape(commandInput)}[/]");
        
        string riskColor = result.Risk switch
        {
            RiskLevel.Critical => "red",
            RiskLevel.High => "red",
            RiskLevel.Medium => "darkorange",
            RiskLevel.Low => "yellow",
            RiskLevel.Safe => "green",
            _ => "grey"
        };
        grid.AddRow("[bold]Risk[/]", $"[{riskColor}]{result.Risk}[/]");

        string confidenceColor = result.Confidence >= 90 ? "green" : result.Confidence >= 70 ? "yellow" : "red";
        grid.AddRow("[bold]Confidence[/]", $"[{confidenceColor}]{result.Confidence}%[/]");
        
        string undoColor = result.UndoPossible ? "green" : "red";
        grid.AddRow("[bold]Undo Possible[/]", $"[{undoColor}]{(result.UndoPossible ? "Yes" : "No")}[/]");

        grid.AddRow("[bold]Estimated Runtime[/]", $"[blue]{result.TotalEstimatedRuntimeMs} ms[/]");

        AnsiConsole.Write(new Panel(grid)
            {
                Border = BoxBorder.Rounded,
                Padding = new Padding(1, 1, 1, 1)
            });

        AnsiConsole.WriteLine();
        AnsiConsole.MarkupLine("[bold]Predicted Effects:[/]");

        if (result.Effects.Count == 0)
        {
            AnsiConsole.MarkupLine("[grey]  No effects predicted or command unknown.[/]");
        }
        else
        {
            var tree = new Tree("Effects");
            
            var groupedEffects = result.Effects.GroupBy(e => e.Category);
            foreach (var group in groupedEffects)
            {
                var categoryNode = tree.AddNode($"[blue]{Markup.Escape(group.Key)}[/]");
                foreach (var effect in group)
                {
                    var effectNode = categoryNode.AddNode(Markup.Escape(effect.Description));
                    
                    if (effect.BeforeState != null || effect.AfterState != null)
                    {
                        var table = new Table()
                            .RoundedBorder()
                            .AddColumn(new TableColumn("[red]Before[/]").Centered())
                            .AddColumn(new TableColumn("[green]After[/]").Centered());

                        table.AddRow(
                            effect.BeforeState != null ? Markup.Escape(effect.BeforeState) : "[grey]none[/]",
                            effect.AfterState != null ? Markup.Escape(effect.AfterState) : "[grey]none[/]"
                        );
                        
                        effectNode.AddNode(table);
                    }
                }
            }

            AnsiConsole.Write(tree);
        }

        if (!string.IsNullOrEmpty(result.AiExplanation))
        {
            AnsiConsole.WriteLine();
            AnsiConsole.Write(new Panel(result.AiExplanation)
            {
                Header = new PanelHeader("[bold magenta]AI Risk Analysis[/]"),
                Border = BoxBorder.Rounded
            }.BorderColor(Color.Magenta));
        }

        AnsiConsole.WriteLine();
    }

    public bool ConfirmExecution(SimulationResult result)
    {
        if (result.Risk >= RiskLevel.High)
        {
            AnsiConsole.MarkupLine($"[bold red]WARNING:[/] This command is classified as [bold]{result.Risk}[/] risk.");
            return AnsiConsole.Confirm("Do you want to execute this command anyway?", defaultValue: false);
        }
        
        return AnsiConsole.Confirm("Do you want to execute this command?", defaultValue: true);
    }
}
