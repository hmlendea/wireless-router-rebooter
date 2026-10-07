# Configuration System

**Files:** `Configuration/BotSettings.cs`, `Configuration/DebugSettings.cs`
**Config File:** `appsettings.json`
**Binding:** `Microsoft.Extensions.Configuration` in `Program.LoadConfiguration()`

## Settings Classes

### BotSettings

```csharp
public sealed class BotSettings
{
    public int PageLoadTimeout { get; set; }
}
```

| Property | Type | Default | Purpose |
|----------|------|---------|---------|
| `PageLoadTimeout` | `int` | 0 (unbound) | Reserved for future use; not currently consumed by any component |

**JSON Section:** `botSettings`

### DebugSettings

```csharp
public sealed class DebugSettings
{
    public string CrashScreenshotFileName { get; set; }
    public bool IsDebugMode { get; set; }

    public bool IsCrashScreenshotEnabled => !string.IsNullOrWhiteSpace(CrashScreenshotFileName);
}
```

| Property | Type | Default | Purpose |
|----------|------|---------|---------|
| `CrashScreenshotFileName` | `string` | `null` | Filename for crash screenshot (e.g., `crash.png`). Relative to log file directory. |
| `IsDebugMode` | `bool` | `false` | Passed to `WebDriverInitialiser.InitialiseAvailableWebDriver()`; affects browser launch mode |
| `IsCrashScreenshotEnabled` | `bool` (computed) | `false` | `true` when `CrashScreenshotFileName` is non-empty |

**JSON Section:** `debugSettings`

### NuciLoggerSettings (from NuciLog)

```csharp
public class NuciLoggerSettings
{
    public string LogFilePath { get; set; }
    public string MinimumLevel { get; set; }
    public bool IsFileOutputEnabled { get; set; }
}
```

| Property | Type | Default | Purpose |
|----------|------|---------|---------|
| `LogFilePath` | `string` | `./logfile.log` | Path to log file (relative or absolute) |
| `MinimumLevel` | `string` | `Info` | Minimum log level: `Trace`, `Debug`, `Info`, `Warn`, `Error`, `Fatal` |
| `IsFileOutputEnabled` | `bool` | `true` | Enable/disable file logging |

**JSON Section:** `nuciLoggerSettings`

## appsettings.json Example

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

## Binding Process

```csharp
static IConfiguration LoadConfiguration() => new ConfigurationBuilder()
    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
    .Build();

// In Main:
IConfiguration config = LoadConfiguration();
config.Bind(nameof(BotSettings), botSettings);
config.Bind(nameof(DebugSettings), debugSettings);
config.Bind(nameof(NuciLoggerSettings), loggingSettings);
```

- `optional: true` — file not required; defaults used if missing
- `reloadOnChange: true` — changes picked up without restart (though app is short-lived)
- `Bind` uses case-insensitive property matching

## Configuration Consumers

| Setting | Consumed By | Purpose |
|---------|-------------|---------|
| `BotSettings.PageLoadTimeout` | None (reserved) | Future: page load timeout for WebProcessor |
| `DebugSettings.IsDebugMode` | `Program.Main` → `WebDriverInitialiser` | Browser debug vs headless mode |
| `DebugSettings.CrashScreenshotFileName` | `Program.SaveCrashScreenshot()` | Screenshot filename on crash |
| `NuciLoggerSettings.*` | `NuciLogger` (via DI) | Logging behaviour |

## Validation

No explicit validation — invalid values use .NET defaults (0, false, null). Consider adding:
- `PageLoadTimeout > 0` validation if consumed
- `LogFilePath` directory existence check
- `MinimumLevel` enum parsing validation

## Extending Configuration

To add new settings:
1. Create `NewSettings.cs` in `Configuration/`
2. Add section to `appsettings.json`
3. Add field in `Program.cs`, bind in `Main`
4. Register in DI: `.AddSingleton(newSettings)`
5. Inject where needed