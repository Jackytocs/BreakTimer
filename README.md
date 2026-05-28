# BreakTimer

A lightweight Windows desktop utility designed to prompt regular breaks, reducing physical strain and eye fatigue during prolonged computer use.

---

## Quick Start

Get started with BreakTimer in seconds:

1. Download the latest release from [GitHub Releases](https://github.com/Jackytocs/BreakTimer/releases)
   - If a published release is not available yet, you can build from source with `dotnet publish` or use the latest artifact from the repo's CI workflow.
2. Run `BreakTimer.exe` (no installation required)
3. Set your work and break durations
4. Click Start and minimize the window to system tray
5. Enjoy automatic break reminders with fullscreen overlays

**Key Features:**

- Multi-monitor support with automatic dimming of all displays
- Precision timing immune to system sleep and CPU load
- Session statistics tracking work and break time
- Privacy-first architecture with zero telemetry
- Lightweight footprint (approximately 10MB RAM)

---

## Features

- **Customizable Intervals**: Adjust work durations (1 to 180 minutes) and break durations (1 to 30 minutes) to suit individual focus cycles.
- **Multi-Monitor Dimming**: Spawns passive background overlays across all secondary displays during breaks to ensure complete eye rest and prevent workflow bypass.
- **Precision Timing**: Utilizes absolute system clock offsets (`DateTime.Now` comparisons) to avoid countdown drift during high CPU load or operating system sleep.
- **Atomic Persistence**: Settings and session history are written to temporary files and replaced atomically, preventing data corruption during unexpected system power-offs.
- **Session Metrics**: Tracks active work hours, completed breaks, and weekly daily averages locally.
- **Privacy First**: Zero cloud integration, telemetry, or remote tracking. All configuration and behavioral logs remain local.
- **Modular and Extensible**: Decoupled core business logic allows easy modifications to styling, sound behaviors, or wellness messages.
- **System Startup Option**: Configures automatic startup with Windows using safe execution path quoting.

---

## System Requirements

- **Operating System**: Windows 10, Windows 11, or Windows 7 with modern runtimes.
- **Runtime**: .NET 8.0 or .NET 9.0 Windows Desktop Runtime.
- **Dependencies**: Newtonsoft.Json (configured via package manager).
- **Resource Footprint**: Minimal memory footprint (approximately 10MB RAM).

---

## Installation

1. Download the latest compiled distribution from the Releases section.
2. Extract the archive contents into your preferred directory (for example, `C:\Program Files\BreakTimer\`).
3. Launch `BreakTimer.exe` to open the main configuration panel.

---

## Usage

### Basic Workflow

1. Configure your desired work and break durations on the main settings panel.
2. Click the **Start** button to run the background service.
3. The configuration window will minimize directly to the Windows system tray.
4. When the work duration is reached, full-screen break overlays cover all active monitors.
5. Relax, review the rotating health and stretching suggestions, and let your eyes rest.
6. When the countdown reaches zero, the break overlays close automatically. Alternatively, use keyboard shortcuts to dismiss the window early.
7. The work timer automatically restarts for the next cycle.

### Keyboard Shortcuts (During Active Breaks)

| Key Combination                   | Action                                                                          |
| :-------------------------------- | :------------------------------------------------------------------------------ |
| **Space** / **Enter** | Skips the current break (triggers a confirmation dialog if enabled in settings) |
| **Escape**                  | Closes the break overlay immediately                                            |

### System Tray Behavior

- **Double-Click**: Restores the Settings window.
- **Right-Click Context Menu**:
  - **Settings**: Opens the configuration and statistics manager.
  - **Test Break**: Immediately previews a 1-minute test break.
  - **Exit**: Terminates the application and stops all timers.

---

## Configuration Settings

The following parameters can be adjusted via the main user interface:

| Setting             | Default Value | Value Range     | Purpose                                                                |
| :------------------ | :------------ | :-------------- | :--------------------------------------------------------------------- |
| Work Duration       | 30 minutes    | 1 - 180 minutes | Total active focus duration before a break triggers.                   |
| Break Duration      | 1 minute      | 1 - 30 minutes  | Duration of the immersive break.                                       |
| Break Screen Theme  | Dark          | Dark / Light    | Color scheme utilized for the overlay screens.                         |
| Sound Notifications | Enabled       | On / Off        | Plays gentle system audio alerts at the start and end of breaks.       |
| Confirm Skip        | Enabled       | On / Off        | Prompt a verification dialog before allowing early break bypass.       |
| Show Statistics     | Enabled       | On / Off        | Toggles the display of current session statistics on the break screen. |
| Auto-Start Windows  | Disabled      | On / Off        | Automates application execution when the system boots.                 |

---

## Data Storage

All persistent information is held locally in the following application data directory:

`%APPDATA%\BreakTimer\`

The directory contains:

- `config.json`: Stores user preferences and runtime validation constraints.
- `statistics.json`: Holds current session data, daily break totals, and historical weekly averages.
- `crash_log.txt`: Written to automatically if a fatal unhandled thread exception occurs.

---

## Extending the Application

The source is structured to facilitate customization:

- **Adding Wellness Tips**: Edit `BreakMessages.cs` to append, modify, or reorganize stretching, hydration, or posture reminders.
- **Custom Themes & Styles**: Adjust the hex colors inside `AppConfig.cs` or extend the color translation logic inside `BreakForm.ApplyTheme()`.
- **Custom Sound Cues**: Update `SoundManager.cs` to trigger localized `.wav` files rather than fallback Windows system sounds.

---

## Technical Architecture

The codebase enforces strict separation of concerns, keeping logical services distinct from GUI rendering routines:

- **Non-Blocking UI UI Execution**: Eliminates thread sleep cycles. Delay transitions use asynchronous tasks to keep the operating system message loop fully responsive.
- **Atomic File Replacement**: Writes updates to a `.tmp` file before replacing active files on the disk, preventing empty settings errors if the system loses power mid-save.
- **Loose Coupling**: Components like `ConfigManager.cs` use event-driven communication to notify the GUI of initialization errors, allowing the storage classes to be unit tested outside of the Windows Forms environment.
- **Unquoted Path Safeguards**: Application startup registrations are wrapped in double quotes in the system registry to prevent unquoted path execution hijack vulnerabilities.

---

## Production Quality & Engineering Practices

This project demonstrates professional software engineering standards suitable for enterprise deployment:

### Code Quality

- Exception handling with global crash logging to prevent unhandled exceptions
- Nullable reference types enabled throughout for compile-time safety
- Proper resource cleanup with using statements and disposal patterns
- Clear separation of concerns between UI and business logic

### Reliability

- Atomic file operations prevent data corruption during system crashes
- DateTime-based timing ensures accuracy even during system sleep or high CPU load
- Graceful error handling for permission issues and resource unavailability
- Comprehensive validation of user input and configuration bounds

### Architecture

- Event-driven error handling decouples core logic from UI framework
- Modular design allows components to be unit tested independently
- Non-blocking async/await patterns keep UI responsive
- Service-based architecture (ConfigManager, SoundManager, SessionStatistics)

### Security

- Local-only data storage with no cloud telemetry or tracking
- Safe registry path handling with proper quoting to prevent hijacking
- User AppData permissions protect configuration from other users
- Zero external dependencies beyond .NET standard libraries

### DevOps Ready

- Configured for GitHub Actions CI/CD automation
- Single-file release builds with no external dependencies
- Professional git workflow with tagged releases
- Comprehensive documentation for contributors

---

## Development & Building from Source

### Prerequisites

- .NET 8.0 SDK or later
- Visual Studio 2022, JetBrains Rider, or VS Code (with C# Dev Kit)

### Building the Project

Run the following commands in your preferred terminal:

```bash
# Clone the repository
git clone https://github.com/Jackytocs/BreakTimer.git
cd BreakTimer

# Restore dependencies
dotnet restore

# Build release artifacts
dotnet build -c Release
```

For a standalone executable without external installers, run:

```bash
dotnet publish src/BreakTimer/BreakTimer.csproj -c Release -r win-x64 --self-contained false -p:PublishSingleFile=true
```

### Repository Structure

```text
BreakTimer/
├── src/
│   ├── Program.cs             # Global crash handling and application initialization
│   ├── AppConfig.cs           # Settings model and boundary validations
│   ├── ConfigManager.cs       # Atomic settings loading and saving engine
│   ├── SessionStatistics.cs   # Metrics engine for tracking daily habits
│   ├── BreakMessages.cs       # Core content repository for physical wellness tips
│   ├── SoundManager.cs        # Audio notification controller
│   ├── SettingsForm.cs        # Main configuration interface and system tray manager
│   ├── BreakForm.cs           # Fullscreen primary and secondary monitor dimming forms
│   └── BreakTimer.csproj      # Build system configurations
├── .gitignore
├── LICENSE
├── README.md
└── BreakTimer.sln
```

---

## Troubleshooting

### The break screen does not display

- Run a test break from the main settings window to verify rendering pipelines.
- Multi-monitor setups are resolved at application launch. If you connected a monitor after starting the program, close and restart BreakTimer.

### Audio notifications do not play

- Confirm that "Sound Notifications" is toggled on inside Settings.
- Verify that the Windows system volume is not muted and that the correct audio output channel is selected.

### Settings fail to save

- Ensure the application has read and write permissions to the `%APPDATA%` environment path.
- If disk space is critically low, the atomic replacement mechanism will reject file modifications to protect configuration integrity.

---

## Limitations

- Native to the Windows operating system; cross-platform deployments are not supported due to the dependency on native Windows Forms libraries and Win32 registry APIs.
- No remote cloud statistics synchronization; progress databases reside strictly on the host hardware.

---

## Contributing

Contributions are welcome. Please refer to [CONTRIBUTING.md](CONTRIBUTING.md) for development rules, style guidelines, and code review criteria.

---

## License

This project is distributed under the terms of the MIT License. Review the [LICENSE](LICENSE) file for more information.
