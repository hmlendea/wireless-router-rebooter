# Security Policy

This document describes the security policy for Wireless Router Rebooter, a self-hosted .NET console utility that automates router reboots via local web interfaces. The project follows coordinated vulnerability disclosure and maintains security updates for the latest release distributed via GitHub Releases.

## 📑 Table of Contents

- [Supported Versions](#supported-versions)
- [Reporting a Vulnerability](#reporting-a-vulnerability)
- [Scope](#scope)
- [Disclosure Policy](#disclosure-policy)
- [Automated Security Scanning](#automated-security-scanning)
- [Safe Harbour](#safe-harbour)
- [Recognition](#recognition)

## 🛡️ Supported Versions

Use this table to indicate which project versions currently receive security maintenance.

| Version | Distribution Channel | Supported |
|---------|--------------------|-----------|
| Latest version | GitHub Releases | ✅ |
| Latest version | Source (main branch) | ✅ |
| Preceding versions | Any distribution channel | ❌ |

## 🚨 Reporting a Vulnerability

Please do not disclose suspected vulnerabilities publicly before maintainers have had an opportunity to validate and remediate them.

To report a vulnerability:
- [GitHub Security Advisories](https://github.com/hmlendea/wireless-router-rebooter/security/advisories)
- Contact the maintainers directly via the repository

## 📌 Scope

The original maintainers are not responsible for changes, modifications, or security issues introduced by downstream forks.

The subsequent report categories are in scope for this repository:
- Credential handling and exposure in logs or memory
- Command injection via router IP address or model arguments
- Path traversal in log file or screenshot paths
- WebDriver/browser automation security issues
- Dependency vulnerabilities in NuGet packages

The subsequent categories are out of scope unless explicitly stated to the contrary:
- Vulnerabilities in the target router's firmware or web interface
- Network-level attacks (MITM, DNS spoofing) on the local network
- Physical access to the machine running the tool
- Operating system or .NET runtime vulnerabilities
- Third-party WebDriver or browser vulnerabilities

## 📢 Disclosure Policy

This project follows coordinated disclosure:
1. Vulnerabilities are investigated privately.
2. A remediation plan is prepared and validated.
3. Public disclosure is published after a fix, mitigation, or agreed risk decision is available.
4. Credit is attributed in accordance with reporter preference and project policy.

## 🔍 Automated Security Scanning

This repository runs automated security scans on every push to `master`, on every pull request targeting `master`, and on a weekly schedule. The scan includes:
- **CodeQL static analysis** For C# sources, using the default CodeQL query suite and uploaded to GitHub Code Scanning
- **Vulnerable package detection** For direct and transitive NuGet dependencies, reported through `dotnet list package --vulnerable --include-transitive`

Results are published to the [Security](https://github.com/hmlendea/wireless-router-rebooter/security) tab and the [Actions](https://github.com/hmlendea/wireless-router-rebooter/actions/workflows/security.yml) workflow page.

## 🧾 Safe Harbour

If your research is conducted in good faith, confined to authorised scope, and disclosed responsibly, the maintainers will not pursue action for policy-compliant activity.

## 🙏 Recognition

We appreciate responsible disclosure. Reporters who desire public attribution may be acknowledged in release notes, advisories, or a dedicated acknowledgements section.