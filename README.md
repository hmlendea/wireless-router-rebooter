[![Donate](https://img.shields.io/badge/-%E2%99%A5%20Donate-%23ff69b4)](https://hmlendea.go.ro/funding)
[![Latest Release](https://img.shields.io/github/v/release/hmlendea/wireless-router-rebooter)](https://github.com/hmlendea/wireless-router-rebooter/releases/latest)
[![Build Status](https://github.com/hmlendea/wireless-router-rebooter/actions/workflows/dotnet.yml/badge.svg)](https://github.com/hmlendea/wireless-router-rebooter/actions/workflows/dotnet.yml)
[![License](https://img.shields.io/github/license/hmlendea/wireless-router-rebooter)](https://github.com/hmlendea/wireless-router-rebooter/blob/master/LICENSE)

# Wireless Router Rebooter

A small .NET console utility that logs into a router web interface and triggers a reboot automatically.

## 📑 Table of Contents

- [Capabilities](#capabilities)
- [Usage](#usage)
- [Command Reference](#command-reference)
- [Known Limitations](#known-limitations)
- [System Requirements](#system-requirements)
- [Installation](#installation)
- [Configuration](#configuration)
- [Architecture](#architecture)
- [Contributing](#contributing)
- [Code of Conduct](#code-of-conduct)
- [License](#license)
- [Security](#security)
- [Privacy](#privacy)

## ✨ Capabilities

- Automates router login via web interface using Selenium WebDriver
- Triggers router reboot programmatically
- Supports multiple router models through a plugin architecture
- Configurable via `appsettings.json` and command-line arguments
- Structured logging with configurable output and levels
- Optional debug mode with crash screenshot capture

## 🚀 Usage

Run the application with credentials:

```bash
dotnet run --project WirelessRouterRebooter.csproj -- --username admin --password your-password
```

If arguments are omitted, the defaults are:

- Username: `admin`
- Password: `admin`
- IP: empty (uses processor default IP)
- Router: `ch7465vf`

Example with custom router and IP:

```bash
dotnet run --project WirelessRouterRebooter.csproj -- --username myuser --password mypass --router f660 --ip 192.168.1.1
```

## ⌨️ Command Reference

| Command | Description |
|---------|-------------|
| `--username` | Router login username |
| `--password` | Router login password |
| `--ip` | Optional custom router IP address; overrides the router model default when set |
| `--router` | Router model (`ch7465vf`, `f660`, `tl-mr105`) |

## ⚠️ Known Limitations

- Only supports Compal CH7465VF, TP-Link MR105, and ZTE F660 routers out of the box
- Requires local network access to the router admin panel
- Requires a local browser supported by Selenium/WebDriver tooling
- No graphical interface; command-line only

## 🖥️ System Requirements

| Component | Minimum | Recommended |
|-----------|---------|-------------|
| .NET SDK | 10.0 | 10.0 |
| Operating System | Linux, macOS, Windows | Linux, macOS, Windows |
| Network | Local access to router admin panel | Local access to router admin panel |
| Browser | Any supported by Selenium/WebDriver | Chrome or Firefox latest |

## 📦 Installation

[![Obtain it from GitHub](https://raw.githubusercontent.com/hmlendea/readme-assets/master/badges/stores/github.png)](https://github.com/hmlendea/wireless-router-rebooter/releases)

### Installation from Source

```bash
git clone https://github.com/hmlendea/wireless-router-rebooter.git
cd wireless-router-rebooter
dotnet restore
dotnet build -c Release
```

### Verification

```bash
dotnet run --project WirelessRouterRebooter.csproj -- --help
```

## ⚙️ Configuration

Configuration is loaded from `appsettings.json`.

### Configuration Files

| File | Scope | Purpose |
|------|-------|---------|
| `appsettings.json` | Application | Main configuration for bot, debug, and logging settings |

### Settings

The subsequent settings are recognised:

| Section | Key | Type | Default | Required | Description |
|---------|-----|------|---------|----------|-------------|
| `botSettings` | `pageLoadTimeout` | `int` | `90` | No | Page load timeout in seconds (reserved for future use) |
| `debugSettings` | `isDebugMode` | `bool` | `false` | No | Enables WebDriver debug mode and crash screenshot capture |
| `debugSettings` | `crashScreenshotFileName` | `string` | `crash.png` | No | File name for crash screenshot when debug mode is enabled |
| `nuciLoggerSettings` | `logFilePath` | `string` | `./logfile.log` | No | Path to log file |
| `nuciLoggerSettings` | `minimumLevel` | `string` | `Info` | No | Minimum log level (Trace, Debug, Info, Warning, Error, Critical) |
| `nuciLoggerSettings` | `isFileOutputEnabled` | `bool` | `true` | No | Enables file output for logs |

Example `appsettings.json`:

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

## 🏗️ Architecture

Wireless Router Rebooter is a .NET console application that automates router reboot via web interface. It uses Selenium WebDriver for browser automation and a plugin architecture for router-specific logic.

See the [architecture documentation](ARCHITECTURE.md) for component diagrams, data flow, and extensibility details.

## 🤝 Contributing

You are welcome to submit any suggestion, feedback, or modification to this project.

When doing so, please:
- Maintain cross-platform compatibility
- Preserve the existing public contract unless a breaking change is intentional
- Submit focused pull requests that conform to the existing code style
- Maintain your branch synchronised with `master`
- Revise the documentation when functionality changes
- Properly test all modifications, including edge cases and error conditions
- Include tests for additional or modified functionality
- Raise a new [issue](https://github.com/hmlendea/wireless-router-rebooter/issues) for problems or suggestions

See the [contributing guidelines](CONTRIBUTING.md) for details on how to report issues, suggest enhancements, and submit changes.

## 📜 Code of Conduct

See the [Code of Conduct](CODE_OF_CONDUCT.md) for the standards of conduct expected in all project spaces.

## 📄 License

This project is being distributed under the `GNU General Public License v3.0 or later`.
See [LICENSE](./LICENSE) for additional information.

## 🔒 Security

Wireless Router Rebooter follows coordinated vulnerability disclosure and maintains security updates for the latest release distributed via GitHub Releases.

For information on reporting security vulnerabilities, see [SECURITY.md](./SECURITY.md).

## 🔒 Privacy

Wireless Router Rebooter is a self-hosted console application. The operator controls all configuration, local storage, logs, backups, access controls, retention, and request handling. The project maintainers do not operate any instance and do not receive any data from self-hosted deployments.

For the detailed description of how the application handles privacy and personal data, see [PRIVACY.md](./PRIVACY.md).

## 💝 Project Engagement

Discovered a problem or have a suggestion? [Open an issue](https://github.com/hmlendea/wireless-router-rebooter/issues)!

If you find this project useful, consider [funding it](https://hmlendea.go.ro/funding) or starring ⭐️ it on GitHub!

[![Donate](https://raw.githubusercontent.com/hmlendea/readme-assets/master/donate_generic.png)](https://hmlendea.go.ro/funding)
