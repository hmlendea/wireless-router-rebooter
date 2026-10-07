<!-- BEGIN PRIVACY TEMPLATE IMMUTABLE -->

# Privacy and Personal Data

This document describes the personal data handling practices of the Wireless Router Rebooter console utility. The application is a self-hosted .NET tool that automates router reboots via local web interfaces. No data is transmitted to project maintainers or external services.

**Information reviewed:** 2026-10-07

## 📑 Table of Contents

- What This Document Covers
- Self-Hosted Deployments
- Data We Handle
- Processing and Use
- Storage, Retention, and Deletion
- External Processing and Integrations
- Data Protection and Security
- Document Changes
- Contact

## 🔎 What This Document Covers

This document describes how Wireless Router Rebooter at https://github.com/hmlendea/wireless-router-rebooter handles personal data. It covers the application behaviour and verified integrations described below. Where the software is self-hosted, the instance operator may have separate responsibilities described below.

## 🏠 Self-Hosted Deployments

Wireless Router Rebooter is a self-hosted console application. The operator (the person running the tool) controls all configuration, local storage, logs, backups, access controls, retention, and request handling. The project maintainers do not operate any instance and do not receive any data from self-hosted deployments.

The application does not send telemetry, crash reports, update checks, or any other data to project maintainers or external services. All network communication is limited to the target router's local web interface.

## 📥 Data We Handle

### Data Provided to the Application

- Router login credentials (username and password) provided via command-line arguments or defaults
- Router IP address provided via command-line argument or processor default
- Router model selection provided via command-line argument

### Data Generated or Collected by the Application

- Application log entries written to the configured log file (default: `./logfile.log`) including startup, operation status, and errors
- Optional crash screenshot saved to the configured file name (default: `crash.png`) when debug mode is enabled and a failure occurs

### Data Received from Integrations

No personal data is received from integrations or third parties.

## 🧭 Processing and Use

The application processes the data described above for these verified functions:
- Router authentication — Router login credentials (username, password)
- Router navigation and reboot trigger — Router IP address, router model
- Operational logging — Log entries (timestamps, operation status, error messages)
- Debug capture — Crash screenshot (when enabled)

## 🗄️ Storage, Retention, and Deletion

- **Log file**: Stored locally at the path configured in `nuciLoggerSettings.logFilePath` (default: `./logfile.log`). The operator controls retention and deletion.
- **Crash screenshot**: Stored locally at the path configured in `debugSettings.crashScreenshotFileName` (default: `crash.png`) when `debugSettings.isDebugMode` is true. The operator controls retention and deletion.
- **Credentials in memory**: Router credentials are held in memory only during the application run and are not persisted to disk by the application.
- The project maintainers do not control storage, deletion, or backups for any self-hosted instance.

## 🔗 External Processing and Integrations

The application has no built-in external data transfer. All network communication is directed exclusively to the target router's local web interface. No external services, recipients, or integrations process or receive data from this application.

| Service or integration | Purpose | Data involved | Configuration or documentation |
|-----------------------|---------|---------------|--------------------------------|
| None | N/A | N/A | N/A |

## 🛡️ Data Protection and Security

- Credentials are passed as command-line arguments and held in memory only for the duration of the application run.
- No credentials are written to log files or persisted to disk by the application.
- The operator is responsible for securing the execution environment, protecting the log file and any crash screenshots, managing network exposure to the router admin panel, and applying updates to the application and its dependencies.
- The application uses Selenium/WebDriver for browser automation; the operator controls the WebDriver configuration and browser profile.
- No encryption at rest is applied to log files or screenshots by the application; the operator should apply filesystem-level protections as appropriate.

## 🔄 Document Changes

Update this document when application data flows, storage, integrations, or deployment responsibilities change. The current version is published at https://github.com/hmlendea/wireless-router-rebooter/blob/main/PRIVACY.md.

## 📬 Contact

For questions about application data handling, contact the project maintainers via the GitHub repository at https://github.com/hmlendea/wireless-router-rebooter. For a self-hosted instance, contact the instance operator. Include the router model and deployment context if relevant; do not send passwords, access tokens, or other secrets.

<!-- END PRIVACY TEMPLATE IMMUTABLE -->