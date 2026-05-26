# Security Policy

## Reporting Security Vulnerabilities

If you discover a security vulnerability in BreakTimer, please do not open a public GitHub issue. Instead, report it responsibly by emailing security concerns with:

- Description of the vulnerability
- Steps to reproduce (if applicable)
- Potential impact
- Any suggested fixes

We take security seriously and will respond promptly to all vulnerability reports.

## Security Features

### Data Protection
- Local-Only Storage: All user data stored locally in %APPDATA%\BreakTimer/
- No Cloud Sync: Zero remote data transmission
- Atomic File Writes: Prevents corruption via temp file and atomic replacement
- No Telemetry: Complete privacy with no user tracking

### Application Security
- Global Exception Handling: Prevents crash dumps exposing sensitive state
- Crash Logging: Errors logged safely to local file (crash_log.txt)
- Registry Security: Paths properly quoted to prevent hijacking
- Permission Checks: Graceful degradation if registry access denied

### Code Quality
- Async/Await: No blocking operations that could hang the application
- Resource Cleanup: All timers and forms properly disposed
- Null Safety: Nullable reference types enabled throughout

## Configuration File Security

### Location
%APPDATA%\BreakTimer\config.json

### Permissions
- File created with default Windows permissions (user-specific)
- Located in user's AppData folder (not system-wide)
- Not accessible to other users on shared systems

### Contents (Non-Sensitive)
Configuration file contains only application settings, no sensitive data.

## Statistics File Security

### Location
%APPDATA%\BreakTimer\statistics.json

### Contents (Non-Sensitive)
- Break counts
- Work time duration
- Daily statistics
- No personally identifying information

## Windows Registry

### Registry Key (Startup Only)
HKEY_CURRENT_USER\SOFTWARE\Microsoft\Windows\CurrentVersion\Run
Value: BreakTimer = "<quoted path to executable>"

### Security Measures
- User-specific key (not system-wide)
- Path always properly quoted (prevents path hijacking)
- Permission validation before write
- Graceful error handling if access denied

## Dependency Security

### Current Dependencies
- System.Drawing: Built-in Windows Forms library
- Newtonsoft.Json: Industry-standard JSON handling
- .NET 8 Runtime: Microsoft-maintained platform

### No External Dependencies
- No npm packages
- No NuGet packages with external dependencies
- Uses only stable, widely-used libraries
- Minimal attack surface

## Build Security

### Release Build Configuration
- PublishSingleFile: true (Single deployable executable)
- PublishReadyToRun: true (AOT-compiled for speed)
- SelfContained: false (Uses system .NET runtime)
- DebugInfo: None (Debug symbols not included in release)

## Known Limitations

### By Design (Not Vulnerabilities)
- No Encryption: Local data not encrypted (but stored in user AppData which is user-protected)
  Rationale: Performance versus security trade-off. User AppData is already user-protected.
- No Update Checking: Manual updates only
  Rationale: Simplicity and avoiding network calls
- No Authentication: Single-user app with no multi-user support
  Rationale: Desktop utility designed for individual users

## Best Practices for Users

1. Keep Windows Updated: Ensures OS-level security patches are applied
2. Keep .NET Updated: Use latest .NET 8 runtime patch versions
3. Run from Trusted Location: Use official GitHub releases only
4. Review Configuration: Regularly check %APPDATA%\BreakTimer/ contents
5. Monitor Registry: Verify only BreakTimer entry in Run key

## Audit Trail

### Crash Logs
Located at: %APPDATA%\BreakTimer\crash_log.txt

Contains:
- Exception messages
- Stack traces
- Timestamps

Can be manually inspected or deleted by user.

## Compliance

- GDPR Compliant: No personal data collection or storage
- CCPA Compliant: No data sharing with third parties
- Privacy-First: Local-only data architecture

---

Last Updated: May 2026

For security concerns, please report responsibly rather than via public issue tracker.
