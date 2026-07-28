# Requirements & Constraints
- **Production-Ready**: The tool must be production-ready.
- **No Mock Data**: It must not use mock data; simulations must be dynamically and accurately evaluated based on the AST and predictors.
- **Perfect Performance**: The simulator must perform perfectly without executing the actual commands.
- **Architecture**:
  - Command Parser -> Simulation Engine -> Predictors (Filesystem, Registry, Processes, Network) -> Dependency Analyzer -> Risk Assessment -> Human-readable Report.
  - Must support Confidence scoring, Risk scoring, Reversibility (Undo analysis).
  - Must provide a Developer API (e.g., `ICommandPredictor`) for new commands.