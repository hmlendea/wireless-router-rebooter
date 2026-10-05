# Build and Release

## Prerequisites

- .NET SDK 10.0+
- Git
- Bash (for release script)

## Build Commands

### Debug Build
```bash
dotnet build -c Debug
```

### Release Build
```bash
dotnet build -c Release
```

### Restore Dependencies
```bash
dotnet restore
```

### Clean
```bash
dotnet clean
```

## Run Commands

### Debug Run
```bash
dotnet run -- --username admin --password admin
```

### Release Run (from built binary)
```bash
dotnet run -c Release -- --username admin --password admin --router ch7465vf
```

### Direct Binary Execution
```bash
./bin/Release/net10.0/WirelessRouterRebooter --username admin --password admin
```

## Test Commands

### Run All Tests
```bash
dotnet test --no-build --verbosity normal
```

### Run with TRX Output
```bash
dotnet test --logger "trx;LogFileName=test_results.trx"
```

### Filter Tests
```bash
dotnet test --filter "FullyQualifiedName~UnitTests"
```

**Note:** No test projects currently exist in the repository.

## Release Process

### Release Script

**File:** `release.sh`

```bash
#!/bin/bash
DOTNET_VERSION="10.0"
RELEASE_SCRIPT_URL="https://raw.githubusercontent.com/hmlendea/deployment-scripts/master/release/dotnet/${DOTNET_VERSION}.sh"
wget --quiet -O - "${RELEASE_SCRIPT_URL}" | bash /dev/stdin ${@}
```

### Usage

```bash
./release.sh [version]
```

- Fetches and executes shared release script from `hmlendea/deployment-scripts`
- Handles version bumping, tagging, GitHub release creation, NuGet packaging
- Arguments passed through to underlying script

### What Release Script Does

1. Validates working directory clean
2. Determines next version (or uses provided)
3. Updates version in `.csproj` / `Directory.Build.props`
4. Commits version bump
5. Creates Git tag
6. Pushes to origin
7. Creates GitHub Release
8. Builds NuGet package (if applicable)
9. Publishes to NuGet.org (if configured)

## CI/CD Pipeline

**File:** `.github/workflows/dotnet.yml`

### Triggers
- Push to `master`
- Pull request to `master`

### Jobs

| Job | Steps |
|-----|-------|
| `build` | Checkout → Setup .NET 10.0 → Restore → Build → Test |

### Workflow Details

```yaml
name: .NET
on:
  push:
    branches: [ master ]
  pull_request:
    branches: [ master ]

jobs:
  build:
    name: Build
    runs-on: ubuntu-latest
    steps:
    - uses: actions/checkout@v2
    - name: Setup .NET
      uses: actions/setup-dotnet@v1
      with:
        dotnet-version: 10.0.x
    - name: Restore dependencies
      run: dotnet restore
    - name: Build
      run: dotnet build --no-restore
    - name: Test
      run: dotnet test --no-build --verbosity normal
```

### Artifacts

No artifacts currently uploaded. Consider adding:
- Built binary for download
- Test results (TRX)
- Code coverage report

## Versioning

- Target framework: `net10.0`
- Version managed by release script
- No `Directory.Build.props` — version in `.csproj` or inferred

## Dependencies

### NuGet Packages

| Package | Version | Purpose |
|---------|---------|---------|
| `Microsoft.Extensions.Configuration` | 10.0.6 | Config binding |
| `Microsoft.Extensions.Configuration.Binder` | 10.0.6 | Object binding |
| `Microsoft.Extensions.Configuration.FileExtensions` | 10.0.6 | File config source |
| `Microsoft.Extensions.Configuration.Json` | 10.0.6 | JSON config |
| `Microsoft.Extensions.DependencyInjection` | 10.0.6 | DI container |
| `NuciCLI.Arguments` | 1.0.1 | CLI parsing |
| `NuciLog` | 1.2.0 | Logging |
| `NuciLog.Core` | 2.6.2 | Logging core |
| `NuciWeb` | 4.0.0 | Web automation core |
| `NuciWeb.Automation` | 1.0.0 | Automation abstraction |
| `NuciWeb.Automation.Selenium` | 1.0.2 | Selenium implementation |

### Updating Dependencies

```bash
dotnet list package --outdated
dotnet add package <PackageName> --version <Version>
```

## Publishing

### Self-Contained (Single File)

```bash
dotnet publish -c Release -r linux-x64 --self-contained true -p:PublishSingleFile=true
```

### Framework-Dependent

```bash
dotnet publish -c Release -r linux-x64 --self-contained false
```

### Supported Runtimes

| RID | Platform |
|-----|----------|
| `linux-x64` | Linux x64 |
| `linux-arm64` | Linux ARM64 |
| `win-x64` | Windows x64 |
| `osx-x64` | macOS x64 |
| `osx-arm64` | macOS ARM64 |

## Troubleshooting

### Build Fails: .NET SDK Not Found
```bash
# Install .NET 10 SDK
# Or use global.json to pin version
```

### WebDriver Issues
- Ensure browser (Chrome/Firefox) installed
- `debugSettings.isDebugMode: true` for visible browser
- Check Selenium version compatibility

### Release Script Fails
- Verify `deployment-scripts` repo accessible
- Check GitHub token permissions (for release creation)
- Ensure git user configured

## Adding Tests

1. Create test project:
   ```bash
   dotnet new xunit -n WirelessRouterRebooter.Tests
   dotnet add WirelessRouterRebooter.Tests reference WirelessRouterRebooter.csproj
   ```

2. Add test dependencies:
   ```bash
   dotnet add WirelessRouterRebooter.Tests package Moq
   dotnet add WirelessRouterRebooter.Tests package Microsoft.NET.Test.Sdk
   ```

3. Run tests:
   ```bash
   dotnet test
   ```