# ZTE F660 Processor

**File:** `Service/Processors/ZteF660.cs`
**Namespace:** `WirelessRouterRebooter.Service.Processors`
**Class:** `ZteF660` (sealed, inherits `RouterProcessor`)
**DI Key:** `"f660"`

## Configuration

| Property | Value |
|----------|-------|
| BrandName | `"ZTE"` |
| ModelName | `"F660"` |
| Default IP | `"192.168.1.1"` |

## Constructor

```csharp
public ZteF660(
    IWebProcessor webProcessor,
    RouterAccessInfo accessInfo)
    : RouterProcessor("ZTE", "F660", "192.168.1.1", accessInfo)
```

## LogIn Implementation

```csharp
public override void LogIn(RouterAccessInfo accessInfo)
{
    webProcessor.GoToUrl($"http://{IpAddress}/");

    webProcessor.SetText(Select.ById("Frm_Username"), accessInfo.Username);
    webProcessor.SetText(Select.ById("Frm_Password"), accessInfo.Password);

    webProcessor.Click(Select.ById("LoginId"));
    webProcessor.WaitForElementToDisappear(Select.ById("LoginId"));
}
```

### Step-by-Step

| Step | Action | Selector | Details |
|------|--------|----------|---------|
| 1 | Navigate to router | `http://{IpAddress}/` | HTTP, default IP `192.168.1.1` |
| 2 | Fill username | `ById("Frm_Username")` | `<input id="Frm_Username">` |
| 3 | Fill password | `ById("Frm_Password")` | `<input id="Frm_Password">` |
| 4 | Submit | `ById("LoginId")` | Login button |
| 5 | Wait for login | `WaitForElementToDisappear(ById("LoginId"))` | Login button disappears on success |

### Selectors

| Element | Selector Type | Value |
|---------|---------------|-------|
| Username field | `ById` | `Frm_Username` |
| Password field | `ById` | `Frm_Password` |
| Login button | `ById` | `LoginId` |

### Login Verification

Uses `WaitForElementToDisappear` on the login button — assumes button is removed/hidden after successful authentication.

## Reboot Implementation

```csharp
public override void Reboot()
{
    webProcessor.GoToUrl($"http://{IpAddress}/getpage.gch?pid=1002&nextpage=manager_dev_conf_t.gch");
    webProcessor.Click(Select.ById("Submit1"));
    webProcessor.AcceptAlert();
    webProcessor.Wait(TimeSpan.FromSeconds(3));
}
```

### Step-by-Step

| Step | Action | Selector | Details |
|------|--------|----------|---------|
| 1 | Navigate to reboot page | Direct URL with query params | `getpage.gch?pid=1002&nextpage=manager_dev_conf_t.gch` |
| 2 | Click reboot button | `ById("Submit1")` | Submit button on reboot page |
| 3 | Accept confirmation | `AcceptAlert()` | JavaScript confirm/alert dialog |
| 4 | Wait | `Wait(3s)` | Wait for reboot to initiate |

### Selectors

| Element | Selector Type | Value |
|---------|---------------|-------|
| Reboot page | Direct URL | `/getpage.gch?pid=1002&nextpage=manager_dev_conf_t.gch` |
| Reboot button | `ById` | `Submit1` |
| Confirmation | `AcceptAlert()` | Browser alert/confirm |

## Timing Summary

| Phase | Wait Strategy |
|-------|---------------|
| Initial page load | Implicit (GoToUrl) |
| Post-login | `WaitForElementToDisappear` (dynamic) |
| Reboot page load | Implicit (GoToUrl) |
| Post-reboot | Fixed `Wait(3s)` |

## Key Differences from Other Processors

| Aspect | Compal | TP-Link | ZTE F660 |
|--------|--------|---------|----------|
| Default IP | 192.168.0.1 | 192.168.0.1 | **192.168.1.1** |
| Login fields | User + Pass | Password only | **User + Pass** |
| Login verification | None | LTE panel visible | **Login button disappears** |
| Reboot navigation | Menu clicks | XPath icon click | **Direct URL** |
| Confirmation | None | Confirm dialog | **JavaScript alert** |
| Wait strategy | Fixed delays | Dynamic waits | **Mixed** |

## Assumptions & Fragility

1. **Different default IP** — `192.168.1.1` vs `192.168.0.1` for others
2. **Direct URL reboot** — Bypasses menu navigation; depends on firmware URL structure
3. **JavaScript alert** — Uses `AcceptAlert()` for confirmation; may vary by firmware
4. **Login button disappearance** — Assumes `LoginId` element removed on success
5. **No HTTPS** — HTTP only
6. **Fixed 3s wait** — No verification reboot actually started

## Testing Notes

- Enable `debugSettings.isDebugMode: true` to observe alert handling
- Direct URL may change with firmware versions
- `AcceptAlert()` throws if no alert present — consider `IsAlertPresent()` check
- Consider verifying reboot initiation (e.g., wait for disconnect)
- Different subnet (192.168.1.x) may require network configuration