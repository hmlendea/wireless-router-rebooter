[![Donate](https://img.shields.io/badge/-%E2%99%A5%20Donate-%23ff69b4)](https://hmlendea.go.ro/fund.html)
[![Latest Release](https://img.shields.io/github/v/release/hmlendea/wireless-router-rebooter)](https://github.com/hmlendea/wireless-router-rebooter/releases/latest)
[![Build Status](https://github.com/hmlendea/wireless-router-rebooter/actions/workflows/dotnet.yml/badge.svg)](https://github.com/hmlendea/wireless-router-rebooter/actions/workflows/dotnet.yml)
[![License: GPL v3](https://img.shields.io/badge/License-GPLv3-blue.svg)](https://gnu.org/licenses/gpl-3.0)

# Wireless Router Rebooter

A small .NET console utility that logs into a router web interface and triggers a reboot automatically.

## What It Does

- Opens the router admin page in an automated browser session.
- Logs in with the provided username and password.
- Navigates to the reboot controls for supported routers.
- Triggers reboot and exits.

## Supported Routers

Currently implemented:

- Compal CH7465VF (default IP: `192.168.0.1`)
- TP-Link MR105 (default IP: `192.168.0.1`)
- ZTE F660 (default IP: `192.168.1.1`)

If your router model is different, you can add a new processor implementation under `Service/Processors`.

## Requirements

- .NET SDK 10.0+
- Network access to your router admin panel
- A local browser supported by Selenium/WebDriver tooling

## Quick Start

1. Clone the repository.
2. Build the project:

```bash
dotnet restore
dotnet build -c Release
```

3. Run with credentials:

```bash
dotnet run --project WirelessRouterRebooter.csproj -- --username admin --password your-password
```

If arguments are omitted, the defaults are:

- Username: `admin`
- Password: `admin`
- IP: empty (use processor default IP)
- Router: `ch7465vf`

## Command-Line Arguments

- `--username`: router login username
- `--password`: router login password
- `--ip`: optional custom router IP address; overrides the router model default when set
- `--router`: router model (`ch7465vf`, `f660`, `tl-mr105`)

Example:

```bash
dotnet run --project WirelessRouterRebooter.csproj -- --username myuser --password mypass --router f660 --ip 192.168.1.1
```

## Configuration

Configuration is loaded from `appsettings.json`.

Example:

```json
{
	"botSettings": {
		"pageLoadTimeout": 90
	},
	"debugSettings": {
		"crashScreenshotFileName": "crash.png",
		"isDebugMode": false
	},
	"nuciLoggerSettings": {
		"logFilePath": "./logfile.log",
		"minimumLevel": "Info",
		"isFileOutputEnabled": true
	}
}
```

Notes:

- `debugSettings.isDebugMode` controls WebDriver initialization mode.
- `nuciLoggerSettings` controls logging behavior and output file path.
- `botSettings.pageLoadTimeout` is present for configuration compatibility and future use.

## Output and Logging

- Logs are written according to `nuciLoggerSettings`.
- By default, file logging is enabled and writes to `./logfile.log`.

## Development

Build:

```bash
dotnet build -c Debug
```

Run:

```bash
dotnet run -- --username admin --password admin
```

## Add Support for Another Router

See [docs/extending.md](docs/extending.md) for detailed steps.

Summary:
1. Create a new class in `Service/Processors/` inheriting from `RouterProcessor`
2. Implement `LogIn(RouterAccessInfo)` and `Reboot()` using `IWebProcessor`
3. Register in `Program.cs` with a unique keyed singleton key
4. Add the key to `ParseDeviceArgument` validation

## Release

A helper script exists for releases:

```bash
./release.sh [version]
```

The script fetches and executes a shared release script from `hmlendea/deployment-scripts` that handles version bumping, tagging, GitHub release creation, and NuGet packaging.

See [docs/build-and-release.md](docs/build-and-release.md) for details.

## Architecture Overview

See [ARCHITECTURE.md](ARCHITECTURE.md) for:
- High-level component diagram
- Data flow description
- Key abstractions and interfaces
- Router processor pattern
- Configuration sections
- Dependencies
- Extensibility guide
- Sequence diagram
- Error handling strategy
- Logging context keys
- Thread safety
- Deployment considerations

## Detailed Documentation

| Document | Description |
|----------|-------------|
| [docs/components/program.md](docs/components/program.md) | Entry point, DI configuration, argument parsing |
| [docs/components/bot-service.md](docs/components/bot-service.md) | Orchestration layer, logging, error handling |
| [docs/components/configuration.md](docs/components/configuration.md) | Settings classes, appsettings.json binding |
| [docs/components/logging.md](docs/components/logging.md) | Custom LogInfoKey and Operation types |
| [docs/components/web-automation.md](docs/components/web-automation.md) | IWebProcessor abstraction, Selenium integration |
| [docs/processors/router-processor-base.md](docs/processors/router-processor-base.md) | Abstract base class, IP resolution |
| [docs/processors/compal-ch7465vf.md](docs/processors/compal-ch7465vf.md) | Compal CH7465VF implementation |
| [docs/processors/tplink-mr105.md](docs/processors/tplink-mr105.md) | TP-Link MR105 implementation |
| [docs/processors/zte-f660.md](docs/processors/zte-f660.md) | ZTE F660 implementation |
| [docs/extending.md](docs/extending.md) | Adding new router processors |
| [docs/build-and-release.md](docs/build-and-release.md) | Build, test, release script, CI/CD |
| [docs/configuration-reference.md](docs/configuration-reference.md) | Complete appsettings.json schema |

## Limitations

- The current implementation targets the HTML structure of supported router firmware pages.
- Firmware UI changes may require selector updates in router processors.
- Only HTTP (not HTTPS) is supported for router admin panels.
- No automatic log rotation; log file grows indefinitely.
- Single-threaded console application; no parallel execution.

## License

Licensed under GPL-3.0-or-later. See `LICENSE` for details.
