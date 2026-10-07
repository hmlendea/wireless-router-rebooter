# TP-Link MR105 Processor

**File:** `Service/Processors/TpLinkMR105Processor.cs`
**Namespace:** `WirelessRouterRebooter.Service.Processors`
**Class:** `TpLinkMR105Processor` (sealed, inherits `RouterProcessor`)
**DI Key:** `"tl-mr105"`

## Configuration

| Property | Value |
|----------|-------|
| BrandName | `"TP-Link"` |
| ModelName | `"MR105"` |
| Default IP | `"192.168.0.1"` |

## Constructor

```csharp
public TpLinkMR105Processor(
    IWebProcessor webProcessor,
    RouterAccessInfo accessInfo)
    : RouterProcessor("TP-Link", "MR105", "192.168.0.1", accessInfo)
```

## LogIn Implementation

```csharp
public override void LogIn(RouterAccessInfo accessInfo)
{
    webProcessor.GoToUrl($"http://{IpAddress}/");

    webProcessor.SetText(Select.ById("pc-login-password"), accessInfo.Password);

    webProcessor.Click(Select.ById("pc-login-btn"));

    string ltePanelXpath = Select.ById("lte_panel");
    string confirmButtonXpath = Select.ByXPath("//*[@id='alert-container']/div/div[@class='position-center-left']/div/div[@class='msg-btn-container']/div/div[2]/button");

    webProcessor.WaitForAnyElementToBeVisible(confirmButtonXpath, ltePanelXpath);

    if (webProcessor.IsElementVisible(confirmButtonXpath))
    {
        webProcessor.Click(confirmButtonXpath);
        webProcessor.WaitForAllElementsToBeVisible(ltePanelXpath);
    }
}
```

### Step-by-Step

| Step | Action | Selector | Details |
|------|--------|----------|---------|
| 1 | Navigate to router | `http://{IpAddress}/` | HTTP |
| 2 | Fill password | `ById("pc-login-password")` | Password-only login (no username field) |
| 3 | Submit | `ById("pc-login-btn")` | Login button |
| 4 | Wait for post-login state | `WaitForAnyElementToBeVisible` | Waits for either confirm dialog or LTE panel |
| 5 | Handle confirm dialog | `ByXPath(...)` | If confirm button visible, click it and wait for LTE panel |

### Selectors

| Element | Selector Type | Value |
|---------|---------------|-------|
| Password field | `ById` | `pc-login-password` |
| Login button | `ById` | `pc-login-btn` |
| LTE panel (success indicator) | `ById` | `lte_panel` |
| Confirm dialog button | `ByXPath` | `//*[@id='alert-container']/div/div[@class='position-center-left']/div/div[@class='msg-btn-container']/div/div[2]/button` |

### Post-Login Logic

The router may show a confirmation dialog after login. The code:
1. Waits for **either** the confirm button **or** the LTE panel to appear
2. If confirm button appears → clicks it → waits for LTE panel
3. If LTE panel appears directly → proceeds (no dialog)

## Reboot Implementation

```csharp
public override void Reboot()
{
    webProcessor.Click(Select.ByXPath("//*[@id='topReboot']/span[@class='icon']"));
    webProcessor.Click(Select.ByXPath("//*[@id='alert-container']/div/div[@class='position-center-left']/div/div[@class='msg-btn-container']/div/div[2]/button"));
    webProcessor.Wait(TimeSpan.FromSeconds(5));
}
```

### Step-by-Step

| Step | Action | Selector | Details |
|------|--------|----------|---------|
| 1 | Click reboot icon | `ByXPath("//*[@id='topReboot']/span[@class='icon']")` | Top menu reboot icon |
| 2 | Confirm reboot | `ByXPath(...same as login confirm...)` | Confirmation dialog button |
| 3 | Wait | `Wait(5s)` | Wait for reboot to initiate |

### Selectors

| Element | Selector Type | Value |
|---------|---------------|-------|
| Reboot icon | `ByXPath` | `//*[@id='topReboot']/span[@class='icon']` |
| Confirm button | `ByXPath` | `//*[@id='alert-container']/div/div[@class='position-center-left']/div/div[@class='msg-btn-container']/div/div[2]/button` |

## Timing Summary

| Phase | Wait Strategy |
|-------|---------------|
| Initial page load | Implicit (GoToUrl) |
| Post-login | `WaitForAnyElementToBeVisible` (dynamic) |
| Confirm dialog | `WaitForAllElementsToBeVisible` (dynamic) |
| Reboot confirmation | Fixed `Wait(5s)` |

## Key Differences from Compal

| Aspect | Compal CH7465VF | TP-Link MR105 |
|--------|-----------------|---------------|
| Login fields | Username + Password | Password only |
| Wait strategy | Fixed `Wait(ms)` | Dynamic `WaitFor*Element*` |
| Menu navigation | Multiple ID clicks | Single XPath click |
| Confirm dialog | Not handled | Handled at login + reboot |

## Assumptions & Fragility

1. **Password-only login** — No username field on login page
2. **Dynamic waits** — Better than fixed delays but depends on element IDs
3. **XPath-heavy** — Reboot and confirm use brittle XPath expressions
4. **Confirm dialog reuse** — Same XPath for login confirm and reboot confirm
5. **LTE panel as success marker** — Assumes `lte_panel` ID indicates logged-in state
6. **No HTTPS** — HTTP only

## Testing Notes

- Enable `debugSettings.isDebugMode: true` to observe dialog handling
- XPath selectors are fragile — prefer ID/class if available
- Confirm dialog may not appear in all firmware versions
- Consider adding explicit wait for reboot completion