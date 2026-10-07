# Extending — Adding New Router Processors

## Overview

The router processor pattern uses keyed dependency injection for runtime selection. Adding a new router requires:

1. Create processor class implementing `IRouterProcessor`
2. Register in DI container with unique key
3. Add key to CLI validation

## Step 1: Create Processor Class

**Location:** `Service/Processors/NewRouterProcessor.cs`

```csharp
using NuciWeb;
using NuciWeb.Automation;
using WirelessRouterRebooter.Service.Models;

namespace WirelessRouterRebooter.Service.Processors
{
    public sealed class NewRouterProcessor(
        IWebProcessor webProcessor,
        RouterAccessInfo accessInfo)
        : RouterProcessor("BrandName", "ModelName", "default.ip.address", accessInfo)
    {
        public override void LogIn(RouterAccessInfo accessInfo)
        {
            webProcessor.GoToUrl($"http://{IpAddress}/");
            // Fill login form
            // Submit
            // Wait for login completion
        }

        public override void Reboot()
        {
            // Navigate to reboot page
            // Trigger reboot
            // Confirm if needed
            // Wait for initiation
        }
    }
}
```

### Required Constructor Parameters

| Parameter | Source | Purpose |
|-----------|--------|---------|
| `IWebProcessor` | DI (transient) | Browser automation |
| `RouterAccessInfo` | DI (singleton) | Credentials + CLI IP override |

### Base Constructor Arguments

```csharp
: RouterProcessor("BrandName", "ModelName", "default.ip.address", accessInfo)
```

| Argument | Description |
|----------|-------------|
| `"BrandName"` | Human-readable brand (e.g., "Netgear") |
| `"ModelName"` | Human-readable model (e.g., "R7000") |
| `"default.ip.address"` | Fallback IP when `--ip` not provided |
| `accessInfo` | Passed to base for IP resolution |

## Step 2: Register in DI Container

**File:** `Program.cs`

```csharp
// Add with other keyed registrations
.AddKeyedSingleton<IRouterProcessor, NewRouterProcessor>("new-router-key")
```

- Key must be lowercase, hyphenated (e.g., `"netgear-r7000"`)
- Used as `--router` argument value

## Step 3: Update CLI Validation

**File:** `Program.cs` — `ParseDeviceArgument` method

```csharp
static string ParseDeviceArgument(ArgumentsCollection arguments)
{
    string device = arguments.Get<string>("router").ToLowerInvariant();

    if (device != "ch7465vf" &&
        device != "f660" &&
        device != "tl-mr105" &&
        device != "new-router-key")  // ADD HERE
    {
        throw new ArgumentException($"Unknown device '{device}'. Valid values are: ch7465vf, f660, tl-mr105, new-router-key");
    }

    return device;
}
```

## Step 4: Test

```bash
dotnet run -- --username admin --password admin --router new-router-key
```

## Selector Discovery (Debug Mode)

1. Enable debug mode in `appsettings.json`:
   ```json
   "debugSettings": { "isDebugMode": true }
   ```

2. Run with new router key — browser opens visibly

3. Use browser DevTools to inspect elements:
   - Right-click → Inspect
   - Note `id`, `name`, `class`, XPath

4. Implement selectors in `LogIn` and `Reboot`

## Best Practices

### Selectors
- Prefer `ById` > `ByName` > `ByCssSelector` > `ByXPath`
- IDs are most stable; XPath is most fragile
- Use `WaitForElementToBeVisible` instead of fixed `Wait(ms)`

### Error Handling
- Let exceptions propagate — `BotService` logs them
- Don't catch/rethrow unless adding context

### Logging
- `BotService` logs `LogIn`/`Reboot` with `BrandName`/`ModelName`/`IpAddress`
- No additional logging needed in processor unless debugging

### Timing
- Use dynamic waits (`WaitFor*`) over fixed delays
- If fixed delays needed, keep minimal and document why

## Example: Minimal Processor Template

```csharp
using NuciWeb;
using NuciWeb.Automation;
using WirelessRouterRebooter.Service.Models;

namespace WirelessRouterRebooter.Service.Processors
{
    public sealed class ExampleRouterProcessor(
        IWebProcessor webProcessor,
        RouterAccessInfo accessInfo)
        : RouterProcessor("Example", "Router", "192.168.1.1", accessInfo)
    {
        public override void LogIn(RouterAccessInfo accessInfo)
        {
            webProcessor.GoToUrl($"http://{IpAddress}/");
            webProcessor.WaitForElementToBeVisible(Select.ById("username"));

            webProcessor.SetText(Select.ById("username"), accessInfo.Username);
            webProcessor.SetText(Select.ById("password"), accessInfo.Password);
            webProcessor.Click(Select.ById("login-btn"));

            webProcessor.WaitForElementToDisappear(Select.ById("login-btn"));
        }

        public override void Reboot()
        {
            webProcessor.GoToUrl($"http://{IpAddress}/reboot");
            webProcessor.WaitForElementToBeVisible(Select.ById("reboot-btn"));
            webProcessor.Click(Select.ById("reboot-btn"));
            webProcessor.AcceptAlert(); // if confirmation dialog
            webProcessor.Wait(TimeSpan.FromSeconds(5));
        }
    }
}
```

## Registration Checklist

- [ ] Processor class created in `Service/Processors/`
- [ ] Inherits `RouterProcessor`
- [ ] Implements `LogIn` and `Reboot`
- [ ] Uses `IWebProcessor` and `RouterAccessInfo` constructor params
- [ ] Added `.AddKeyedSingleton<IRouterProcessor, NewProcessor>("key")` in `Program.cs`
- [ ] Key added to `ParseDeviceArgument` validation
- [ ] Key added to `ArgumentException` message
- [ ] Tested with `dotnet run -- --router key`
- [ ] Documented in `docs/processors/` (optional but recommended)

## Common Pitfalls

| Issue | Cause | Fix |
|-------|-------|-----|
| "Unknown device" error | Key not in `ParseDeviceArgument` | Add to validation list |
| DI resolution fails | Key mismatch | Ensure key matches exactly (case-sensitive in DI, lowercased in validation) |
| Login hangs | Wrong selector / no wait | Use `WaitForElementToBeVisible` |
| Reboot doesn't trigger | Wrong navigation / missing confirm | Verify URL, check for alert dialog |
| Crash screenshot not saved | `CrashScreenshotFileName` empty | Set in `appsettings.json` |