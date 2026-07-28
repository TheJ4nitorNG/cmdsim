using System;
using System.Collections.Generic;
using System.Linq;
using CmdSim.Engine.Core;
using CmdSim.Sdk.Interfaces;
using CmdSim.Engine.Parsing;
using CmdSim.Engine.Predictors.Filesystem;
using CmdSim.Engine.Predictors.Registry;
using CmdSim.Engine.Predictors.Services;
using CmdSim.Engine.Predictors.Environment;
using CmdSim.Engine.Predictors.Network;

using CmdSim.Engine.AI;

namespace CmdSim.ConsoleApp;

public class Program
{
    public static void Main(string[] args)
    {
        if (args.Length == 0)
        {
            Console.WriteLine("Usage: CmdSim.Console <command>");
            return;
        }

        string input = string.Join(" ", args);

        var parser = new CommandParser();
        var predictors = new List<ICommandPredictor>
        {
            new RemoveItemPredictor(),
            new NewItemPredictor(),
            new MoveItemPredictor(),
            new RenameItemPredictor(),
            new NewItemPropertyPredictor(),
            new StopServicePredictor(),
            new SetItemEnvPredictor(),
            new InvokeWebRequestPredictor(),
            new InvokeRestMethodPredictor()
        };

        var engine = new SimulationEngine(parser, predictors);
        var result = engine.Simulate(input);

        var aiModule = new AiReasoningModule(new LocalHeuristicAiProvider());
        aiModule.Analyze(result);

        var reportGen = new ReportGenerator();
        reportGen.Render(result, input);

        bool confirmed = reportGen.ConfirmExecution(result);
        if (confirmed)
        {
            Console.WriteLine("\n[Execution would happen here...]");
        }
        else
        {
            Console.WriteLine("\n[Execution aborted]");
        }
    }
}
