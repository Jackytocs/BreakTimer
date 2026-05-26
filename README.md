
# BreakTimer

[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](LICENSE)
[![.NET 8.0](https://img.shields.io/badge/.NET-8.0-512BD4)](https://dotnet.microsoft.com/en-us/download/dotnet/8.0)
[![Windows](https://img.shields.io/badge/Platform-Windows%2010%2B-0078D4)](https://www.microsoft.com/windows)
[![Build Status](https://img.shields.io/badge/Build-Passing-brightgreen)](.github/workflows/build.yml)

A lightweight Windows desktop utility designed to prompt regular breaks, reducing physical strain and eye fatigue during prolonged computer use.

---

## Quick Start

Get started with BreakTimer in seconds:

1. Download the latest release from [GitHub Releases](https://github.com/yourusername/BreakTimer/releases)
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

- Customizable work and break intervals (1-180 minutes and 1-30 minutes)
- Multi-monitor support with automatic display dimming
- Precision timing that handles system sleep
- Session statistics tracking
- Dark and light themes
- Sound notifications
- Auto-start with Windows
- Privacy-first: all data stored locally, no telemetry

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

Simple, clean design:

- Responsive UI with async/await (never blocks)
- Safe data storage with atomic file writes
- Event-based error handling
- Secure registry path handling
- Independent components for easy testing

---

## Development & Building from Source

### Prerequisites

- .NET 8.0 SDK or later
- Visual Studio 2022, JetBrains Rider, or VS Code (with C# Dev Kit)

### Building the Project

Run the following commands in your preferred terminal:

```bash
# Clone the repository
git clone https://github.com/yourusername/BreakTimer.git
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
├── Program.cs             # Application entry point
├── AppConfig.cs           # Settings model
├── ConfigManager.cs       # Settings persistence
├── SessionStatistics.cs   # Break tracking
├── BreakMessages.cs       # Wellness tips
├── SoundManager.cs        # Audio alerts
├── SettingsForm.cs        # Main UI window
├── BreakForm.cs           # Break overlay
├── BreakTimer.csproj      # Project file
├── .gitignore
├── LICENSE
└── README.md
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
