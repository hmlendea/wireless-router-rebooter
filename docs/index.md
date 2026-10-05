# Documentation

This directory contains detailed technical documentation for the Wireless Router Rebooter project.

## Contents

| Document | Description |
|----------|-------------|
| [components/program.md](components/program.md) | Entry point, DI configuration, argument parsing, startup/shutdown |
| [components/bot-service.md](components/bot-service.md) | `IBotService` / `BotService` orchestration, logging, error handling |
| [components/configuration.md](components/configuration.md) | Settings classes, `appsettings.json` binding, validation |
| [components/logging.md](components/logging.md) | Custom `LogInfoKey` and `Operation` types, structured logging |
| [components/web-automation.md](components/web-automation.md) | `IWebProcessor` abstraction, Selenium integration via NuciWeb |
| [processors/router-processor-base.md](processors/router-processor-base.md) | `RouterProcessor` abstract base class, IP resolution |
| [processors/compal-ch7465vf.md](processors/compal-ch7465vf.md) | Compal CH7465VF implementation details |
| [processors/tplink-mr105.md](processors/tplink-mr105.md) | TP-Link MR105 implementation details |
| [processors/zte-f660.md](processors/zte-f660.md) | ZTE F660 implementation details |
| [extending.md](extending.md) | Adding new router processors, registration, testing |
| [build-and-release.md](build-and-release.md) | Build, test, release script, CI/CD pipeline |
| [configuration-reference.md](configuration-reference.md) | Complete `appsettings.json` schema with all options |

## Quick Navigation

- **Architecture overview**: See root [`ARCHITECTURE.md`](../ARCHITECTURE.md)
- **User guide**: See root [`README.md`](../README.md)
- **Adding a router**: See [extending.md](extending.md)
- **Configuration**: See [configuration-reference.md](configuration-reference.md)