# Track 002: Filesystem Simulation, Reports, and Scoring

## Objective
- Simulate core filesystem cmdlets
- Generate human-readable report
- Confidence scoring
- Risk scoring

## Tasks
- [x] Implement core filesystem predictors (`Remove-Item`, `New-Item`, `Move-Item`, `Rename-Item`).
- [x] Integrate Confidence scoring into the filesystem predictors and engine.
- [x] Integrate Risk scoring into predictors based on sensitive paths (e.g., C:\Windows, System32).
- [x] Build the human-readable report generator using `Spectre.Console` in the Console project.
- [x] Validate implementation with robust test coverage.

## Telemetry Target
We will track implementation accuracy and completeness. We will take as many turns as necessary to avoid placeholders, mock data, or prototype logic, ensuring 100% production-ready code on the first attempt.