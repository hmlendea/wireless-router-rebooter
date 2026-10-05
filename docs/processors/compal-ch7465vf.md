# Compal CH7465VF Processor

**File:** `Service/Processors/CompalCH7465VF.cs`
**Namespace:** `WirelessRouterRebooter.Service.Processors`
**Class:** `CompalCH7465VF` (sealed, inherits `RouterProcessor`)
**DI Key:** `"ch7465vf"` (default)

## Configuration

| Property | Value |
|----------|-------|
| BrandName | `"Compal"` |
| ModelName | `"CH7465VF"` |
| Default IP | `"192.168.0.1"` |

## Constructor

```csharp
public CompalCH7465VF(
    IWebProcessor webProcessor,
    RouterAccessInfo accessInfo)
    : RouterProcessor("Compal", "CH7465VF", "192.168.0.1", accessInfo)
```

## LogIn Implementation

```csharp
public override void LogIn(RouterAccessInfo accessInfo)
{
    webProcessor.GoToUrl($"http://{IpAddress}/");
    webProcessor.Wait(5000);

    webProcessor.SetText(Select.ByName("loginUsername"), accessInfo.Username);
    webProcessor.SetText(Select.ByName("loginPassword"), accessInfo.Password);

    webProcessor.Click(Select.ById("c_42"));
}
```

### Step-by-Step

| Step | Action | Selector | Details |
|------|--------|----------|---------|
| 1 | Navigate to router | `http://{IpAddress}/` | HTTP (not HTTPS) |
| 2 | Wait | `Wait(5000)` | 5 second fixed delay for page load |
| 3 | Fill username | `ByName("loginUsername")` | `<input name="loginUsername">` |
| 4 | Fill password | `ByName("loginPassword")` | `<input name="loginPassword">` |
| 5 | Submit | `ById("c_42")` | Login button `<button id="c_42">` |

### Selectors

| Element | Selector Type | Value |
|---------|---------------|-------|
| Username field | `ByName` | `loginUsername` |
| Password field | `ByName` | `loginPassword` |
| Login button | `ById` | `c_42` |

## Reboot Implementation

```csharp
public override void Reboot()
{
    webProcessor.Wait(5000);

    for (int i = 0; i < 3; i++)
    {
        webProcessor.Click(Select.ById("c_mu25"));
        webProcessor.Wait(250);
    }

    webProcessor.Wait(1000);

    for (int i = 0; i < 3; i++)
    {
        webProcessor.Click(Select.ById("c_mu27"));
        webProcessor.Wait(250);
    }

    webProcessor.Click(Select.ById("c_rr14"));
}
```

### Step-by-Step

| Step | Action | Selector | Details |
|------|--------|----------|---------|
| 1 | Wait | `Wait(5000)` | 5s after login |
| 2 | Click menu | `ById("c_mu25")` ×3 | Navigate menu (3 clicks, 250ms each) |
| 3 | Wait | `Wait(1000)` | 1s delay |
| 4 | Click submenu | `ById("c_mu27")` ×3 | Navigate to reboot section (3 clicks, 250ms each) |
| 5 | Trigger reboot | `ById("c_rr14")` | Reboot button |

### Selectors

| Element | Selector Type | Value | Purpose |
|---------|---------------|-------|---------|
| Main menu item | `ById` | `c_mu25` | Top-level menu (clicked 3x) |
| Submenu item | `ById` | `c_mu27` | Reboot submenu (clicked 3x) |
| Reboot button | `ById` | `c_rr14` | Final reboot trigger |

## Timing Summary

| Phase | Total Wait |
|-------|------------|
| Initial page load | 5000ms |
| Post-login | 5000ms |
| Menu navigation | 3 × 250ms = 750ms |
| Submenu delay | 1000ms |
| Submenu navigation | 3 × 250ms = 750ms |
| **Total** | **~12.5 seconds** |

## Assumptions & Fragility

1. **Fixed delays** — No explicit wait for element state; relies on timing
2. **Menu structure** — Assumes specific menu IDs (`c_mu25`, `c_mu27`) and click counts
3. **HTTP only** — No HTTPS support
4. **No login verification** — Doesn't confirm login succeeded before reboot
5. **Firmware-specific** — Selectors tied to specific firmware version

## Testing Notes

- Enable `debugSettings.isDebugMode: true` to watch browser interaction
- Selectors may change with firmware updates
- Consider adding `WaitForElementToBeVisible` for critical elements
- No logout step — session ends when browser quits