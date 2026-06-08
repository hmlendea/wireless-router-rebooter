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

## Command-Line Arguments

- `--username`: router login username
- `--password`: router login password

Example:

```bash
dotnet run --project WirelessRouterRebooter.csproj -- --username myuser --password mypass
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

1. Create a new class implementing `IRouterProcessor` in `Service/Processors`.
2. Implement:
	 - `LogIn(UserCredentials userCredentials)`
	 - `Reboot()`
3. Register it in dependency injection in `Program.cs`.

## Release

A helper script exists for releases:

```bash
./release.sh
```

## Limitations

- The current implementation targets the HTML structure of supported router firmware pages.
- Firmware UI changes may require selector updates in router processors.

## License

Licensed under GPL-3.0-or-later. See `LICENSE` for details.
