# BotService — Orchestration Layer

**File:** `Service/BotService.cs`
**Namespace:** `WirelessRouterRebooter.Service`
**Class:** `BotService` (implements `IBotService`)

## Responsibility

High-level workflow orchestration: coordinates router login and reboot operations with structured logging and error handling.

## Interface

```csharp
public interface IBotService
{
    void Run(RouterAccessInfo accessInfo);
}
```

## Constructor Injection

```csharp
public BotService(
    IRouterProcessor routerProcessor,
    ILogger logger)
```

- `routerProcessor` — keyed singleton resolved in `Program.cs` based on `--router` argument
- `logger` — `NuciLogger` for structured logging

## Method: Run(RouterAccessInfo)

```csharp
public void Run(RouterAccessInfo accessInfo)
{
    LogIn(accessInfo);
    Reboot();
}
```

Sequential execution: login must succeed before reboot is attempted.

## Method: LogIn(RouterAccessInfo)

```csharp
void LogIn(RouterAccessInfo accessInfo)
{
    string ipAddress = accessInfo.GetIpAddressOrDefault(routerProcessor.IpAddress);

    logger.Info(MyOperation.LogIn, OperationStatus.Started,
        LogInfo(MyLogInfoKey.IpAddress, ipAddress),
        LogInfo(MyLogInfoKey.BrandName, routerProcessor.BrandName),
        LogInfo(MyLogInfoKey.ModelName, routerProcessor.ModelName),
        LogInfo(MyLogInfoKey.Username, accessInfo.Username));

    try
    {
        routerProcessor.LogIn(accessInfo);

        logger.Debug(MyOperation.LogIn, OperationStatus.Success, ...);
    }
    catch (Exception ex)
    {
        logger.Error(MyOperation.LogIn, OperationStatus.Failure, ex, ...);
        throw;
    }
}
```

### Logging Structure

| Level | Operation | Status | Context Keys |
|-------|-----------|--------|--------------|
| Info | `LogIn` | `Started` | IpAddress, BrandName, ModelName, Username |
| Debug | `LogIn` | `Success` | Same as above |
| Error | `LogIn` | `Failure` | Same + exception |

### IP Resolution

Uses `RouterAccessInfo.GetIpAddressOrDefault(routerProcessor.IpAddress)`:
- CLI `--ip` argument takes precedence
- Falls back to processor's default IP (`192.168.0.1` for Compal/TP-Link, `192.168.1.1` for ZTE)

## Method: Reboot()

```csharp
void Reboot()
{
    logger.Info(MyOperation.Reboot, OperationStatus.Started,
        LogInfo(MyLogInfoKey.IpAddress, routerProcessor.IpAddress),
        LogInfo(MyLogInfoKey.BrandName, routerProcessor.BrandName),
        LogInfo(MyLogInfoKey.ModelName, routerProcessor.ModelName));

    try
    {
        routerProcessor.Reboot();

        logger.Debug(MyOperation.Reboot, OperationStatus.Success, ...);
    }
    catch (Exception ex)
    {
        logger.Error(MyOperation.Reboot, OperationStatus.Failure, ex, ...);
        throw;
    }
}
```

### Logging Structure

| Level | Operation | Status | Context Keys |
|-------|-----------|--------|--------------|
| Info | `Reboot` | `Started` | IpAddress, BrandName, ModelName |
| Debug | `Reboot` | `Success` | Same as above |
| Error | `Reboot` | `Failure` | Same + exception |

## Error Handling

- Exceptions from `routerProcessor.LogIn()` or `routerProcessor.Reboot()` are:
  1. Logged at `Error` level with `OperationStatus.Failure`
  2. Re-thrown to caller (`Program.Run`)
- No retry logic — fails fast
- Caller (`Program.Run`) catches, logs `Fatal`, saves crash screenshot, quits WebDriver

## Dependencies

- `IRouterProcessor` — router-specific implementation (keyed singleton)
- `ILogger` — `NuciLogger` for structured logging
- `RouterAccessInfo` — credentials + IP (passed per-call)
- `MyOperation` — `LogIn`, `Reboot` operation identifiers
- `MyLogInfoKey` — `IpAddress`, `BrandName`, `ModelName`, `Username` log keys
- `NuciLog.Core` — `OperationStatus`, `LogInfo`

## Invariants

1. `LogIn` always called before `Reboot`
2. Same `routerProcessor` instance used for both operations
3. Exceptions propagate to caller — no silent failures
4. Structured logging captures router identity for every operation