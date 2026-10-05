# RouterProcessor — Abstract Base Class

**File:** `Service/Processors/RouterProcessor.cs`
**Namespace:** `WirelessRouterRebooter.Service.Processors`
**Class:** `RouterProcessor` (abstract, implements `IRouterProcessor`)

## Purpose

Base class for all router-specific processors. Provides common fields, IP resolution logic, and enforces the `IRouterProcessor` contract.

## Constructor

```csharp
protected RouterProcessor(
    string brandName,
    string modelName,
    string defaultIpAddress,
    RouterAccessInfo accessInfo)
{
    BrandName = brandName;
    ModelName = modelName;
    IpAddress = string.IsNullOrWhiteSpace(accessInfo?.IpAddress)
        ? defaultIpAddress
        : accessInfo.IpAddress;
}
```

| Parameter | Purpose |
|-----------|---------|
| `brandName` | Human-readable brand (e.g., "Compal") |
| `modelName` | Human-readable model (e.g., "CH7465VF") |
| `defaultIpAddress` | Fallback IP when CLI `--ip` not provided |
| `accessInfo` | Contains CLI-provided IP (may be null/empty) |

## Properties

| Property | Type | Setter | Description |
|----------|------|--------|-------------|
| `BrandName` | `string` | init-only | Router brand for logging/display |
| `ModelName` | `string` | init-only | Router model for logging/display |
| `IpAddress` | `string` | init-only | Resolved IP (CLI override or default) |

## IP Resolution Logic

```csharp
IpAddress = string.IsNullOrWhiteSpace(accessInfo?.IpAddress)
    ? defaultIpAddress
    : accessInfo.IpAddress;
```

- CLI `--ip` argument → `RouterAccessInfo.IpAddress`
- If provided (non-empty), uses CLI value
- Otherwise uses processor's `defaultIpAddress`
- Resolution happens once at construction time

## Abstract Methods (Must Implement)

```csharp
public abstract void LogIn(RouterAccessInfo accessInfo);
public abstract void Reboot();
```

### LogIn(RouterAccessInfo)

- Navigate to router login page
- Fill username/password from `accessInfo`
- Submit login form
- Wait for login completion
- **Must use** `accessInfo.Username` and `accessInfo.Password`

### Reboot()

- Navigate to reboot page/section
- Trigger reboot action
- Confirm reboot if required
- Wait for reboot initiation
- **Uses** `this.IpAddress` (already resolved)

## Inheritance Pattern

```csharp
public sealed class SpecificRouterProcessor(
    IWebProcessor webProcessor,
    RouterAccessInfo accessInfo)
    : RouterProcessor("Brand", "Model", "default.ip", accessInfo)
{
    public override void LogIn(RouterAccessInfo accessInfo) { ... }
    public override void Reboot() { ... }
}
```

- Constructor receives `IWebProcessor` (DI) and `RouterAccessInfo` (DI singleton)
- Passes brand, model, default IP to base
- Implements router-specific automation logic

## Default IPs by Processor

| Processor | Brand | Model | Default IP |
|-----------|-------|-------|------------|
| `CompalCH7465VF` | Compal | CH7465VF | `192.168.0.1` |
| `TpLinkMR105Processor` | TP-Link | MR105 | `192.168.0.1` |
| `ZteF660` | ZTE | F660 | `192.168.1.1` |

## DI Registration (in Program.cs)

```csharp
.AddKeyedSingleton<IRouterProcessor, CompalCH7465VF>("ch7465vf")
.AddKeyedSingleton<IRouterProcessor, TpLinkMR105Processor>("tl-mr105")
.AddKeyedSingleton<IRouterProcessor, ZteF660>("f660")
.AddSingleton(sp => sp.GetRequiredKeyedService<IRouterProcessor>(deviceKey))
```

- Keyed singletons allow runtime selection
- Final registration resolves the selected key to `IRouterProcessor` (non-keyed)
- `BotService` receives the resolved `IRouterProcessor`

## Invariants

1. `IpAddress` resolved once at construction — immutable thereafter
2. `BrandName`/`ModelName` used for structured logging in `BotService`
3. `LogIn` called before `Reboot` by `BotService.Run`
4. Same `RouterAccessInfo` instance passed to both methods
5. Exceptions not caught — propagate to `BotService` for logging

## Extending

To create a new processor:
1. Create `NewRouterProcessor.cs` in `Service/Processors/`
2. Inherit `RouterProcessor`
3. Implement `LogIn` and `Reboot` using injected `IWebProcessor`
4. Register in `Program.cs` with unique key
5. Add key to `ParseDeviceArgument` validation