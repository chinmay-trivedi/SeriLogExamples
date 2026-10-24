# SeriLogExamples

Serilog examples for different types of .NET projects (console apps, Web APIs including minimal APIs, EF Core, and .NET Framework) with different sinks: File, MSSqlServer, Seq, EventLog, Email and EmailPickup.

These projects show how to use Serilog in several kinds of .NET apps:
- How to separate the logs based on either log level or properties.
- How to read appsettings.json config file for SeriLog
- How to use different type of sinks
- How to use log template with different sinks
  

## Projects

| Project | What it shows |
|---|---|
| `SeriLogConsoleApp` | Console app configured through `appsettings.json` |
| `SeriLogNet8.ConsoleApp` | .NET 8 console app with a service, custom email body formatter and several sinks |
| `SeriLogNetFrameWork` | .NET Framework 4.7.2 console app |
| `SeriLogMinimalWebAPI` | Minimal API |
| `SeriLogWebAPI` | Web API with request-logging middleware, action/page filters and log-context enrichment |
| `SerilogNet8.WebAPI` | .NET 8 Web API |
| `SeriLogWithEFCore` | Logging EF Core database operations |
| `SeriLogWithEFFilter` | EF Core logging with filters |

## Getting Started

These instructions will get you a copy of the project up and running on your local machine for development and testing purposes.

### Prerequisites

- Visual Studio 2022 or later, or the .NET CLI
- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0) or later
- .NET Framework 4.7.2 developer pack (only for `SeriLogNetFrameWork`, Windows only)

### NuGet packages

These are restored automatically on build; you do not need to install them manually.

- Serilog.Extensions.Logging
- Serilog.Expressions
- Serilog.Settings.Configuration
- Serilog.Enrichers.Context
- Serilog.Enrichers.Environment
- Serilog.Enrichers.Process
- Serilog.Enrichers.Thread
- Serilog.Sinks.File
- Serilog.Sinks.MSSqlServer
- Serilog.Sinks.Seq
- Serilog.Sinks.EventLog
- Serilog.Sinks.Email
- Serilog.Sinks.EmailPickup

### Installing

1. Clone the repository.
2. Open the solution in Visual Studio.
3. Build the solution.
4. Run the application.

## Running Seq locally

A `docker-compose.yml` is included to start [Seq](https://datalust.co/seq) for the Seq sink examples:

```bash
docker compose up -d
```

Seq is then available at http://localhost:5341 (UI and ingestion). Authentication is disabled, so use it for local development only. Point the Seq sink at it with `"serverUrl": "http://localhost:5341"` in the project's Serilog configuration.

## Built With

- Visual Studio 2022
- .NET 8.0
- Console App Project template
- SeriLog.Extensions.Logging

## License

This project is licensed under the MIT License - see the LICENSE file for details.

## Acknowledgments

- Kudos to SeriLog team
