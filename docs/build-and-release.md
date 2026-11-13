# Build and Release

## Prerequisites

- .NET SDK 10.0+
- Git

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

Publishing a GitHub Release triggers the release workflow, which builds and uploads the release artefacts.

### GitHub Release Workflow

**File:** `.github/workflows/github-release.yml`

```yaml
name: GitHub Release
on:
  release:
    types: [ published ]

permissions:
  contents: write
```

### What the Release Workflow Does

1. Checks out the repository with full history
2. Determines the .NET version from `WirelessRouterRebooter/WirelessRouterRebooter.csproj`
3. Sets up the matching .NET SDK
4. Derives the version from the release tag (stripping the `v` prefix)
5. Normalises the `origin` remote URL to HTTPS
6. Builds the release artefacts through the shared release script from `hmlendea/dotnet-release-scripts`
7. Publishes the artefacts to the GitHub Release with `softprops/action-gh-release`

### Release Artefacts

The shared release script publishes self-contained builds for the subsequent runtime identifiers and packages them as `wireless-router-rebooter_<version>_<rid>.zip`:

- `linux-arm`
- `linux-arm64`
- `linux-x64`
- `osx-arm64`
- `osx-x64`
- `win-arm64`
- `win-x64`

## CI/CD Pipeline

**Files:** `.github/workflows/dotnet.yml`, `.github/workflows/github-release.yml`

### Triggers

| Workflow | Triggers |
|----------|----------|
| `.github/workflows/dotnet.yml` | Push to `master`, pull request to `master` |
| `.github/workflows/github-release.yml` | Published GitHub Release |

### Jobs

| Workflow | Job | Steps |
|----------|-----|-------|
| `.github/workflows/dotnet.yml` | `build` | Checkout → Setup .NET 10.0 → Restore → Build → Test |
| `.github/workflows/github-release.yml` | `release` | Checkout → Determine .NET version → Setup .NET → Extract version → Normalise remote → Build artefacts → Publish release assets |

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
| `linux-arm` | Linux ARM |
| `linux-arm64` | Linux ARM64 |
| `linux-x64` | Linux x64 |
| `osx-arm64` | macOS ARM64 |
| `osx-x64` | macOS x64 |
| `win-arm64` | Windows ARM64 |
| `win-x64` | Windows x64 |

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

### Release Workflow Fails
- Verify the `hmlendea/dotnet-release-scripts` repository is accessible
- Check that the release workflow has `contents: write` permission
- Ensure the release tag is a semantic version prefixed with `v`
- Confirm the release assets glob matches the artefacts in `WirelessRouterRebooter/bin/Release/`

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