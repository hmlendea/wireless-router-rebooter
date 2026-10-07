# Program.cs — Entry Point

**File:** `Program.cs`
**Namespace:** `WirelessRouterRebooter`
**Class:** `Program` (sealed, static `Main`)

## Responsibility

Application bootstrap: configuration loading, CLI parsing, DI container construction, WebDriver initialisation, service execution, and graceful shutdown.

## Execution Flow

```text
Main(args)
├── LoadConfiguration() → IConfiguration
├── Bind settings: BotSettings, DebugSettings, NuciLoggerSettings
├── ParseArguments(args) → ArgumentsCollection
├── RetrieveAccessInfo() → RouterAccessInfo
├── ParseDeviceArgument() → deviceKey (validated)
├── WebDriverInitialiser.InitialiseAvailableWebDriver(debugSettings.IsDebugMode) → IWebDriver
├── Build DI container (ServiceCollection)
├── Resolve ILogger, log StartUp
├── Run(serviceProvider, accessInfo)
│   ├── Resolve IBotService
│   ├── botService.Run(accessInfo)
│   │   ├── LogIn(accessInfo)
│   │   └── Reboot()
│   └── Catch exceptions → log Fatal, SaveCrashScreenshot()
└── webDriver.Quit() (finally)
```

## Key Methods

| Method | Purpose |
|--------|---------|
| `Main(string[])` | Entry point; orchestrates entire lifecycle |
| `LoadConfiguration()` | Builds `ConfigurationBuilder` with `appsettings.json` (optional, reloadOnChange) |
| `ParseArguments(string[])` | Uses `NuciCLI.Arguments.ArgumentParser`; defines 4 args with defaults |
| `ParseDeviceArgument(ArgumentsCollection)` | Validates router key against `ch7465vf`, `f660`, `tl-mr105` |
| `RetrieveAccessInfo(ArgumentsCollection)` | Maps CLI args → `RouterAccessInfo` |
| `Run(IServiceProvider, RouterAccessInfo)` | Executes bot service, handles exceptions, saves crash screenshot |
| `SaveCrashScreenshot()` | If `DebugSettings.IsCrashScreenshotEnabled`, saves WebDriver screenshot to log directory |

## DI Registrations

```csharp
new ServiceCollection()
    .AddSingleton(botSettings)
    .AddSingleton(debugSettings)
    .AddSingleton(loggingSettings)
    .AddSingleton(accessInfo)
    .AddSingleton<ILogger, NuciLogger>()
    .AddSingleton(webDriver)
    .AddTransient<IWebProcessor, SeleniumWebProcessor>()
    .AddKeyedSingleton<IRouterProcessor, CompalCH7465VF>("ch7465vf")
    .AddKeyedSingleton<IRouterProcessor, TpLinkMR105Processor>("tl-mr105")
    .AddKeyedSingleton<IRouterProcessor, ZteF660>("f660")
    .AddSingleton(sp => sp.GetRequiredKeyedService<IRouterProcessor>(deviceKey))
    .AddSingleton<IBotService, BotService>()
    .BuildServiceProvider();
```

- **Keyed singletons** for router processors enable runtime selection via `deviceKey`
- `IWebProcessor` is transient (new instance per resolution)
- `RouterAccessInfo` is singleton (same credentials for login + reboot)

## Argument Definitions

| Argument | Required | Default | Description |
|----------|----------|---------|-------------|
| `--username` | No | `admin` | Router login username |
| `--password` | No | `admin` | Router login password |
| `--ip` | No | `""` | Custom IP (overrides processor default) |
| `--router` | No | `ch7465vf` | Router model key |

## Error Handling

- `ArgumentException` for invalid router key (thrown during `ParseDeviceArgument`)
- `AggregateException` inner exceptions logged individually
- All other exceptions logged as `Fatal` with `OperationStatus.Failure`
- Crash screenshot saved when `DebugSettings.IsCrashScreenshotEnabled`
- `webDriver.Quit()` always called in `finally` block

## Dependencies

- `Microsoft.Extensions.Configuration` — JSON config binding
- `NuciCLI.Arguments` — CLI parsing
- `NuciWeb.Automation.Selenium.WebDriverInitialiser` — WebDriver factory
- `NuciLog` — `ILogger`, `NuciLogger`
- Custom: `BotSettings`, `DebugSettings`, `RouterAccessInfo`, `IBotService`, `IRouterProcessor`, `IWebProcessor`