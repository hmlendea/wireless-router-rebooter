# Logging System

**Files:** `Logging/MyLogInfoKey.cs`, `Logging/MyOperation.cs`
**Logger:** `NuciLog.NuciLogger` (implements `NuciLog.Core.ILogger`)
**Configuration:** `NuciLoggerSettings` via `appsettings.json`

## Custom Log Keys — MyLogInfoKey

```csharp
public sealed class MyLogInfoKey : LogInfoKey
{
    MyLogInfoKey(string name) : base(name) { }

    public static LogInfoKey RouterProcessor => new MyLogInfoKey(nameof(RouterProcessor));
    public static LogInfoKey Username => new MyLogInfoKey(nameof(Username));
    public static LogInfoKey BrandName => new MyLogInfoKey(nameof(BrandName));
    public static LogInfoKey ModelName => new MyLogInfoKey(nameof(ModelName));
    public static LogInfoKey IpAddress => new MyLogInfoKey(nameof(IpAddress));
}
```

| Key | Value Type | Used In | Purpose |
|-----|------------|---------|---------|
| `RouterProcessor` | `string` | (reserved) | Router processor type identifier |
| `Username` | `string` | `LogIn` | Login username |
| `BrandName` | `string` | `LogIn`, `Reboot` | Router brand (Compal, TP-Link, ZTE) |
| `ModelName` | `string` | `LogIn`, `Reboot` | Router model (CH7465VF, MR105, F660) |
| `IpAddress` | `string` | `LogIn`, `Reboot` | Target router IP address |

Inherits from `NuciLog.Core.LogInfoKey` — enables structured key-value logging.

## Custom Operations — MyOperation

```csharp
public sealed class MyOperation : Operation
{
    MyOperation(string name) : base(name) { }

    public static Operation ParseArguments => new MyOperation(nameof(ParseArguments));
    public static Operation LogIn => new MyOperation(nameof(LogIn));
    public static Operation Reboot => new MyOperation(nameof(Reboot));
}
```

| Operation | Used In | Purpose |
|-----------|---------|---------|
| `ParseArguments` | `Program.ParseArguments` | CLI parsing phase (logged in `Program.Main`) |
| `LogIn` | `BotService.LogIn` | Router authentication phase |
| `Reboot` | `BotService.Reboot` | Router reboot trigger phase |

Inherits from `NuciLog.Core.Operation` — enables operation-scoped logging with status.

## Logging Patterns in BotService

### LogIn Flow

```csharp
// Start
logger.Info(MyOperation.LogIn, OperationStatus.Started,
    LogInfo(MyLogInfoKey.IpAddress, ipAddress),
    LogInfo(MyLogInfoKey.BrandName, routerProcessor.BrandName),
    LogInfo(MyLogInfoKey.ModelName, routerProcessor.ModelName),
    LogInfo(MyLogInfoKey.Username, accessInfo.Username));

// Success
logger.Debug(MyOperation.LogIn, OperationStatus.Success, ...same keys...);

// Failure
logger.Error(MyOperation.LogIn, OperationStatus.Failure, exception, ...same keys...);
```

### Reboot Flow

```csharp
// Start
logger.Info(MyOperation.Reboot, OperationStatus.Started,
    LogInfo(MyLogInfoKey.IpAddress, routerProcessor.IpAddress),
    LogInfo(MyLogInfoKey.BrandName, routerProcessor.BrandName),
    LogInfo(MyLogInfoKey.ModelName, routerProcessor.ModelName));

// Success
logger.Debug(MyOperation.Reboot, OperationStatus.Success, ...same keys...);

// Failure
logger.Error(MyOperation.Reboot, OperationStatus.Failure, exception, ...same keys...);
```

## Log Levels Used

| Level | NuciLog Method | When |
|-------|----------------|------|
| `Info` | `logger.Info()` | Operation start (LogIn, Reboot) |
| `Debug` | `logger.Debug()` | Operation success |
| `Error` | `logger.Error()` | Operation failure + exception |
| `Fatal` | `logger.Fatal()` | Unhandled exception in `Program.Run` |

## Log Output Format

Controlled by `NuciLoggerSettings`:
- `LogFilePath` — `./logfile.log` (default)
- `MinimumLevel` — `Info` (default)
- `IsFileOutputEnabled` — `true` (default)

Example log entry:
```
[2026-10-05 10:30:45.123] INFO  [LogIn:Started] IpAddress=192.168.0.1 BrandName=Compal ModelName=CH7465VF Username=admin
[2026-10-05 10:30:50.456] DEBUG [LogIn:Success] IpAddress=192.168.0.1 BrandName=Compal ModelName=CH7465VF Username=admin
[2026-10-05 10:30:55.789] INFO  [Reboot:Started] IpAddress=192.168.0.1 BrandName=Compal ModelName=CH7465VF
[2026-10-05 10:31:00.012] DEBUG [Reboot:Success] IpAddress=192.168.0.1 BrandName=Compal ModelName=CH7465VF
```

## Crash Screenshot Integration

When exception occurs in `Program.Run`:
1. `logger.Fatal(Operation.Unknown, OperationStatus.Failure, exception)`
2. `SaveCrashScreenshot()` called if `DebugSettings.IsCrashScreenshotEnabled`
3. Screenshot saved to same directory as log file with `CrashScreenshotFileName`

## Extending Logging

To add new log context:
1. Add key to `MyLogInfoKey`
2. Use `LogInfo(MyLogInfoKey.NewKey, value)` in log calls
3. Key appears in structured log output automatically

To add new operation:
1. Add operation to `MyOperation`
2. Use `logger.Info(MyOperation.NewOp, OperationStatus.Started, ...)`
3. Follow Started/Success/Failure pattern