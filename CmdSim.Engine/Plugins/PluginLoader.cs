using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using CmdSim.Sdk.Interfaces;

namespace CmdSim.Engine.Plugins;

public class PluginLoader
{
    public IEnumerable<ICommandPredictor> LoadPredictors(string directoryPath)
    {
        if (!Directory.Exists(directoryPath))
        {
            yield break;
        }

        var dllFiles = Directory.GetFiles(directoryPath, "*.dll");
        var interfaceType = typeof(ICommandPredictor);

        foreach (var file in dllFiles)
        {
            Assembly assembly;
            try
            {
                // In a true robust scenario, we'd use AssemblyLoadContext. 
                // For this implementation, Assembly.LoadFrom suffices for local plugin DLLs.
                assembly = Assembly.LoadFrom(file);
            }
            catch (Exception)
            {
                // Skip files that aren't valid .NET assemblies or can't be loaded
                continue;
            }

            Type[] types;
            try
            {
                types = assembly.GetTypes();
            }
            catch (ReflectionTypeLoadException e)
            {
                types = e.Types.Where(t => t != null).ToArray()!;
            }

            foreach (var type in types)
            {
                if (interfaceType.IsAssignableFrom(type) && type.IsClass && !type.IsAbstract)
                {
                    ICommandPredictor? predictor = null;
                    try
                    {
                        predictor = Activator.CreateInstance(type) as ICommandPredictor;
                    }
                    catch
                    {
                        // Skip if we can't instantiate (e.g., no default constructor)
                    }

                    if (predictor != null)
                    {
                        yield return predictor;
                    }
                }
            }
        }
    }
}
