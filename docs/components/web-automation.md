# Web Automation Layer

**Interface:** `NuciWeb.Automation.IWebProcessor`
**Implementation:** `NuciWeb.Automation.Selenium.SeleniumWebProcessor`
**Factory:** `NuciWeb.Automation.Selenium.WebDriverInitialiser`
**DI Registration:** Transient `IWebProcessor` → `SeleniumWebProcessor`

## Abstraction: IWebProcessor

Defined in `NuciWeb.Automation` package. Key methods used by router processors:

| Method | Purpose |
|--------|---------|
| `GoToUrl(string url)` | Navigate to URL |
| `SetText(Select selector, string text)` | Fill input field |
| `Click(Select selector)` | Click element |
| `Wait(int milliseconds)` | Fixed delay |
| `Wait(TimeSpan timespan)` | Fixed delay |
| `WaitForElementToDisappear(Select selector)` | Wait until element not visible |
| `WaitForAnyElementToBeVisible(params Select[] selectors)` | Wait for any of multiple elements |
| `WaitForAllElementsToBeVisible(params Select[] selectors)` | Wait for all elements |
| `IsElementVisible(Select selector)` | Check element visibility |
| `AcceptAlert()` | Accept JavaScript alert/confirm |

## Selector Factory: Select

Static factory for element locators:

| Method | Locator Type |
|--------|--------------|
| `ById(string id)` | `id` attribute |
| `ByName(string name)` | `name` attribute |
| `ByXPath(string xpath)` | XPath expression |
| `ByClassName(string className)` | CSS class |
| `ByCssSelector(string selector)` | CSS selector |
| `ByTagName(string tagName)` | HTML tag |
| `ByLinkText(string text)` | Exact link text |
| `ByPartialLinkText(string text)` | Partial link text |

## WebDriver Initialisation

```csharp
// In Program.cs
webDriver = WebDriverInitialiser.InitialiseAvailableWebDriver(debugSettings.IsDebugMode);
```

- `IsDebugMode = false` (default): Headless browser, optimised for automation
- `IsDebugMode = true`: Visible browser, useful for debugging selectors

Returns `OpenQA.Selenium.IWebDriver` — also registered as singleton in DI.

## DI Registration

```csharp
.AddTransient<IWebProcessor, SeleniumWebProcessor>()
.AddSingleton(webDriver)
```

- `IWebProcessor` transient: new wrapper per resolution (stateless)
- `IWebDriver` singleton: single browser session shared across processors

## Usage in Router Processors

Each processor receives `IWebProcessor` via constructor injection:

```csharp
public sealed class CompalCH7465VF(
    IWebProcessor webProcessor,
    RouterAccessInfo accessInfo)
    : RouterProcessor("Compal", "CH7465VF", "192.168.0.1", accessInfo)
{
    public override void LogIn(RouterAccessInfo accessInfo)
    {
        webProcessor.GoToUrl($"http://{IpAddress}/");
        webProcessor.Wait(5000);
        webProcessor.SetText(Select.ByName("loginUsername"), accessInfo.Username);
        webProcessor.SetText(Select.ByName("loginPassword"), accessInfo.Password);
        webProcessor.Click(Select.ById("c_42"));
    }
    // ...
}
```

## Selector Strategies by Router

### Compal CH7465VF
- Login: `ByName("loginUsername")`, `ByName("loginPassword")`, `ById("c_42")`
- Reboot: `ById("c_mu25")` (3x), `ById("c_mu27")` (3x), `ById("c_rr14")`
- Uses fixed `Wait(ms)` delays

### TP-Link MR105
- Login: `ById("pc-login-password")`, `ById("pc-login-btn")`
- Post-login: `WaitForAnyElementToBeVisible` for confirm dialog or LTE panel
- Reboot: XPath for reboot icon and confirm button
- Uses `Wait(TimeSpan.FromSeconds(5))`

### ZTE F660
- Login: `ById("Frm_Username")`, `ById("Frm_Password")`, `ById("LoginId")`
- Post-login: `WaitForElementToDisappear(ById("LoginId"))`
- Reboot: Direct URL navigation to reboot page, `ById("Submit1")`, `AcceptAlert()`
- Uses `Wait(TimeSpan.FromSeconds(3))`

## Error Handling

- `IWebProcessor` methods throw on element not found, timeout, stale element
- Exceptions propagate to `BotService` → logged with `OperationStatus.Failure`
- No retry at web automation level — fails fast

## Browser Lifecycle

1. `Program.Main` → `WebDriverInitialiser.InitialiseAvailableWebDriver()`
2. Single `IWebDriver` instance shared for entire application lifetime
3. `webDriver.Quit()` called in `finally` block in `Program.Run()`
4. Crash screenshot captured via `((ITakesScreenshot)webDriver).GetScreenshot()` if enabled

## Dependencies

- `NuciWeb.Automation` — `IWebProcessor`, `Select`, `WebDriverInitialiser`
- `NuciWeb.Automation.Selenium` — `SeleniumWebProcessor` implementation
- `OpenQA.Selenium` — `IWebDriver`, `ITakesScreenshot`
- `Microsoft.Extensions.DependencyInjection` — DI registration

## Testing Considerations

- No unit tests for web automation (integration only)
- Selectors are brittle — firmware UI changes break automation
- Debug mode (`IsDebugMode=true`) allows manual selector verification
- Consider page object pattern for complex routers