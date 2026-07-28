# Project Goal
We are building a command line tool (PowerShell Command Simulator) that will simulate PowerShell commands and show the predicted results (filesystem changes, registry edits, process creation, network activity, etc.) before execution.

# Tech Stack
- **Language**: C# (.NET 9+)
- **Runtime**: .NET
- **CLI**: PowerShell Module + Native Console App
- **Parser**: PowerShell AST (`System.Management.Automation.Language`)
- **UI**: Spectre.Console
- **Plugin System**: MEF or AssemblyLoadContext
- **Serialization**: System.Text.Json
- **Logging**: Serilog
- **Dependency Injection**: Microsoft.Extensions.DependencyInjection
- **Testing**: xUnit + FluentAssertions
- **Benchmarking**: BenchmarkDotNet
- **Packaging**: PowerShell Gallery + winget + NuGet