# Configuration Reference

Complete `appsettings.json` schema with all supported options.

## Full Schema

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

## Section: botSettings

**Class:** `WirelessRouterRebooter.Configuration.BotSettings`
**JSON Key:** `botSettings`
**Required:** No (all optional)

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `pageLoadTimeout` | `integer` | `0` | Reserved for future use. Intended for page load timeout in seconds. Not currently consumed by any component. |

### Example
```json
"botSettings": {
  "pageLoadTimeout": 90
}
```

### Notes
- Value of `0` means no timeout (current behavior)
- When implemented, would likely be passed to `IWebProcessor` navigation methods
- Consider validation: `pageLoadTimeout >= 0`

---

## Section: debugSettings

**Class:** `WirelessRouterRebooter.Configuration.DebugSettings`
**JSON Key:** `debugSettings`
**Required:** No (all optional)

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `crashScreenshotFileName` | `string` | `null` | Filename for crash screenshot. Saved to same directory as log file. Empty = disabled. |
| `isDebugMode` | `boolean` | `false` | Controls WebDriver initialisation mode. `true` = visible browser for debugging. `false` = headless/optimised. |

### Computed Property (Not in JSON)
```csharp
public bool IsCrashScreenshotEnabled => !string.IsNullOrWhiteSpace(CrashScreenshotFileName);
```

### Examples

**Production (minimal):**
```json
"debugSettings": {
  "isDebugMode": false
}
```

**Development (with crash screenshots):**
```json
"debugSettings": {
  "crashScreenshotFileName": "crash.png",
  "isDebugMode": true
}
```

**CI/CD (headless, with crash capture):**
```json
"debugSettings": {
  "crashScreenshotFileName": "crash.png",
  "isDebugMode": false
}
```

### Notes
- `crashScreenshotFileName` is relative to log file directory (`Path.GetDirectoryName(logFilePath)`)
- `isDebugMode` passed directly to `WebDriverInitialiser.InitialiseAvailableWebDriver()`
- When `isDebugMode=true`, browser window visible — useful for selector debugging
- Crash screenshot captured on any unhandled exception in `Program.Run()`

---

## Section: nuciLoggerSettings

**Class:** `NuciLog.Configuration.NuciLoggerSettings` (from NuciLog package)
**JSON Key:** `nuciLoggerSettings`
**Required:** No (all optional)

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `logFilePath` | `string` | `"./logfile.log"` | Path to log file. Relative to working directory or absolute. |
| `minimumLevel` | `string` | `"Info"` | Minimum log level. Values: `Trace`, `Debug`, `Info`, `Warn`, `Error`, `Fatal`. Case-insensitive. |
| `isFileOutputEnabled` | `boolean` | `true` | Enable/disable file logging. Console output controlled separately by NuciLog. |

### Log Levels (Verbosity Order)
```
Trace (most verbose)
Debug
Info
Warn
Error
Fatal (least verbose)
```

### Examples

**Production:**
```json
"nuciLoggerSettings": {
  "logFilePath": "./logfile.log",
  "minimumLevel": "Info",
  "isFileOutputEnabled": true
}
```

**Debug/Development:**
```json
"nuciLoggerSettings": {
  "logFilePath": "./logfile.log",
  "minimumLevel": "Debug",
  "isFileOutputEnabled": true
}
```

**Minimal (errors only):**
```json
"nuciLoggerSettings": {
  "logFilePath": "./logfile.log",
  "minimumLevel": "Error",
  "isFileOutputEnabled": true
}
```

**Disable file logging:**
```json
"nuciLoggerSettings": {
  "isFileOutputEnabled": false
}
```

### Notes
- `logFilePath` directory must exist or be creatable
- `minimumLevel` parsed by NuciLog — invalid values may default to `Info`
- File logging appends by default
- Structured logging includes: timestamp, level, operation, status, key-value pairs

---

## Environment Variable Overrides

Configuration supports environment variables via `ConfigurationBuilder` (not explicitly configured but available):

| Setting | Environment Variable |
|---------|---------------------|
| `botSettings:pageLoadTimeout` | `BOTSETTINGS__PAGELOADTIMEOUT` |
| `debugSettings:crashScreenshotFileName` | `DEBUGSETTINGS__CRASHSCREENSHOTFILENAME` |
| `debugSettings:isDebugMode` | `DEBUGSETTINGS__ISDEBUGMODE` |
| `nuciLoggerSettings:logFilePath` | `NUCILOGGERSETTINGS__LOGFILEPATH` |
| `nuciLoggerSettings:minimumLevel` | `NUCILOGGERSETTINGS__MINIMUMLEVEL` |
| `nuciLoggerSettings:isFileOutputEnabled` | `NUCILOGGERSETTINGS__ISFILEOUTPUTENABLED` |

Use double underscore `__` for nested keys (standard .NET Configuration).

---

## Configuration Loading Code

**File:** `Program.cs`

```csharp
static IConfiguration LoadConfiguration() => new ConfigurationBuilder()
    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
    .Build();

// Binding in Main:
IConfiguration config = LoadConfiguration();
config.Bind(nameof(BotSettings), botSettings);
config.Bind(nameof(DebugSettings), debugSettings);
config.Bind(nameof(NuciLoggerSettings), loggingSettings);
```

### Binding Behaviour
- `optional: true` — file not required; uses class defaults if missing
- `reloadOnChange: true` — watches file for changes (limited utility for short-lived console app)
- `Bind` — case-insensitive property matching, ignores extra JSON properties

---

## Validation Rules (Recommended)

Currently no validation — invalid values use .NET defaults. Recommended additions:

### BotSettings
```csharp
// In BotSettings constructor or after Bind:
if (PageLoadTimeout < 0) PageLoadTimeout = 0;
```

### DebugSettings
```csharp
// Validate crash screenshot filename
if (!string.IsNullOrWhiteSpace(CrashScreenshotFileName))
{
    var invalidChars = Path.GetInvalidFileNameChars();
    if (CrashScreenshotFileName.IndexOfAny(invalidChars) >= 0)
        throw new ArgumentException("Invalid crash screenshot filename");
}
```

### NuciLoggerSettings
```csharp
// Validate log level
var validLevels = new[] { "Trace", "Debug", "Info", "Warn", "Error", "Fatal" };
if (!validLevels.Contains(MinimumLevel, StringComparer.OrdinalIgnoreCase))
    MinimumLevel = "Info";

// Validate log file path
if (IsFileOutputEnabled && string.IsNullOrWhiteSpace(LogFilePath))
    LogFilePath = "./logfile.log";
```

---

## Minimal Working Configuration

```json
{}
```

All settings optional — application runs with defaults.

---

## Complete Example with All Options

```json
{
  "botSettings": {
    "pageLoadTimeout": 60
  },
  "debugSettings": {
    "crashScreenshotFileName": "crash.png",
    "isDebugMode": false
  },
  "nuciLoggerSettings": {
    "logFilePath": "/var/log/wireless-router-rebooter.log",
    "minimumLevel": "Info",
    "isFileOutputEnabled": true
  }
}
```

---

## File Locations

| File | Purpose |
|------|---------|
| `appsettings.json` | Primary config (copied to output directory) |
| `bin/Debug/net10.0/appsettings.json` | Debug build output |
| `bin/Release/net10.0/appsettings.json` | Release build output |

**Copy behavior:** `<CopyToOutputDirectory>PreserveNewest</CopyToOutputDirectory>` in `.csproj`