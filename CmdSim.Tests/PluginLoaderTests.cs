using System;
using System.IO;
using System.Linq;
using CmdSim.Engine.Plugins;
using CmdSim.Sdk.Interfaces;
using FluentAssertions;
using Xunit;

namespace CmdSim.Tests;

public class PluginLoaderTests
{
    [Fact]
    public void LoadPredictors_FromDirectory_FindsAndInstantiatesPredictors()
    {
        // Arrange
        // The Engine DLL should be in the current execution directory during test
        string currentDir = AppDomain.CurrentDomain.BaseDirectory;
        var loader = new PluginLoader();

        // Act
        var predictors = loader.LoadPredictors(currentDir).ToList();

        // Assert
        // We know the Engine contains several built-in predictors (RemoveItemPredictor, etc.)
        // So we expect to find at least those if it successfully loads the DLLs in the directory.
        predictors.Should().NotBeEmpty();
        predictors.Should().Contain(p => p.GetType().Name == "RemoveItemPredictor");
    }
}
