# Developer Guide

This guide explains the architecture, design patterns, and development practices for BreakTimer.

## Architecture Overview

The application follows a layered architecture:

```
Presentation Layer
├─ SettingsForm.cs      (Main window and settings UI)
└─ BreakForm.cs         (Break screen UI)

Business Logic Layer
├─ SessionStatistics.cs (Break tracking and history)
├─ BreakMessages.cs     (Health tips management)
└─ SoundManager.cs      (Audio notifications)

Data Layer
├─ AppConfig.cs         (Settings model)
└─ ConfigManager.cs     (Persistence)

Entry Point
└─ Program.cs           (Application startup)
```

## Core Components

### Program.cs

Entry point that initializes the application.

- Initializes global ConfigManager singleton
- Creates and displays the main window
- Enables nullable reference type checking

### AppConfig.cs

Data model for application settings.

Properties:
- WorkDurationMinutes, BreakDurationMinutes: Timer durations
- SelectedTheme: Dark or Light
- SoundNotificationsEnabled: Audio alerts
- ConfirmBeforeSkip: Require confirmation for skip
- ShowStatisticsOnBreak: Display stats during break
- StartWithWindows: Auto-start at login
- Color settings: Background, text, and accent colors

Methods:
- Validate(): Ensures values are within allowed ranges
- Clone(): Creates a copy of the configuration

### ConfigManager.cs

Handles JSON persistence of settings and statistics.

Properties:
- Config: Current AppConfig instance
- Statistics: Current SessionStatistics instance

Methods:
- LoadConfiguration(): Reads config.json from AppData
- SaveConfiguration(): Writes AppConfig to JSON
- LoadStatistics(): Reads statistics.json from AppData
- SaveStatistics(): Writes SessionStatistics to JSON
- ResetSessionStatistics(): Clears session data

Data location: %APPDATA%\BreakTimer\

### SessionStatistics.cs

Tracks break sessions and historical data.

Methods:
- RecordBreak(): Logs a completed break
- UpdateWorkTime(seconds): Adds work time to current session
- GetSessionSummary(): Returns today's break count and work time
- GetWeeklyAverageBreaks(): Calculates average breaks over 7 days

Properties track:
- Current session breaks taken
- Daily work and break time
- Historical data for each day

### BreakMessages.cs

Static class containing health-focused messages for breaks.

Message categories (21 messages total):
- Eye Care: Tips for resting eyes
- Hydration: Reminders to drink water
- Stretching: Simple exercises
- Posture: Corrections for sitting posture
- Motivation: Encouraging messages

Methods:
- GetAllMessages(): Returns all messages
- GetRandomMessage(): Returns a random message
- GetMessageByCategory(category): Returns messages from a specific category

### SoundManager.cs

Handles audio notifications using Windows system sounds.

Methods:
- PlayBreakStartSound(): Notifies user break is starting
- PlayBreakEndSound(): Notifies user break is ending
- PlayBeep(): Simple beep notification

Uses SystemSounds from Windows Forms. Fails silently if unavailable.

### SettingsForm.cs

Main application window for configuration and control.

Features:
- Work and break duration input fields
- Theme selection dropdown
- Checkbox toggles for options
- Statistics display
- Test Break button
- Reset Statistics button
- Help button
- System tray integration

Timers:
- workTimer: Counts down work duration
- trayUpdateTimer: Updates tray tooltip every second

Registry operations for startup automation:

```
HKEY_CURRENT_USER\SOFTWARE\Microsoft\Windows\CurrentVersion\Run
```

### BreakForm.cs

Full-screen break notification window.

Features:
- Countdown timer display
- Animated health messages
- Message positioning in all screen corners
- Theme application (dark/light)
- Statistics display option
- Keyboard shortcuts

Keyboard Shortcuts:
- Space or Enter: Skip break
- Esc: Close break screen

Timers:
- breakTimer: Counts down break duration
- animationTimer: Moves messages around screen

## Design Patterns Used

### Singleton Pattern

ConfigManager is accessed globally through Program.ConfigManager:

```csharp
var config = Program.ConfigManager.Config;
Program.ConfigManager.SaveConfiguration();
```

This ensures single instance and consistent state across the application.

### Static Helpers

BreakMessages and SoundManager are static classes providing utility functions without instantiation.

### Event-Driven UI

Windows Forms controls respond to events:
- Timer.Tick: Handle countdown and animations
- Button.Click: Handle user actions
- Form.Load/FormClosing: Initialize and cleanup

### Guard Clauses

Null safety checks before accessing nullable fields:

```csharp
if (someControl != null)
{
    someControl.Text = "value";
}
```

The project has `<Nullable>enable</Nullable>` enabled in the project file.

## Data Flow

1. **Application Startup**
   - Program.cs runs Main()
   - ConfigManager loads settings from JSON
   - SettingsForm is created and displayed

2. **User Configuration**
   - User adjusts settings in SettingsForm
   - Settings are validated
   - Settings persist to config.json

3. **Work Session**
   - User clicks Start
   - workTimer begins countdown
   - SettingsForm minimizes to tray
   - Work continues in background

4. **Break Trigger**
   - workTimer reaches zero
   - BreakForm is displayed full-screen
   - Sound plays if enabled
   - Statistics recorded

5. **Break Session**
   - Animation timer moves messages around
   - Messages display health tips
   - breakTimer counts down
   - User can skip or wait for completion

6. **Break Completion**
   - breakTimer reaches zero
   - Sound plays if enabled
   - Statistics updated and saved
   - BreakForm closes
   - Work timer resets and continues

## File Structure

```
BreakTimer/
├── Program.cs                 - Startup and global config
├── AppConfig.cs              - Settings data model
├── ConfigManager.cs          - JSON persistence
├── SessionStatistics.cs      - Break tracking
├── BreakMessages.cs          - Health tips
├── SoundManager.cs           - Audio playback
├── SettingsForm.cs           - Main UI
├── BreakForm.cs              - Break screen UI
├── BreakTimer.csproj         - Project file
├── README.md                 - User documentation
└── DEVELOPER.md              - This file
```

## Configuration File Format

config.json structure:

```json
{
  "workDurationMinutes": 30,
  "breakDurationMinutes": 1,
  "selectedTheme": "dark",
  "soundNotificationsEnabled": true,
  "confirmBeforeSkip": true,
  "showStatisticsOnBreak": true,
  "startWithWindows": false,
  "breakScreenBackgroundColor": "#141414",
  "breakScreenTextColor": "#FFFFFF",
  "accentColor": "#87CEEB"
}
```

statistics.json structure:

```json
{
  "breaksTakenToday": 5,
  "totalWorkTimeSeconds": 1800,
  "totalBreakTimeSeconds": 300,
  "dailyStats": {
    "2024-05-21": {
      "date": "2024-05-21",
      "breaksTaken": 5,
      "workTimeMinutes": 30,
      "breakTimeMinutes": 5
    }
  }
}
```

## Building and Running

### Debug Build

```
dotnet build
dotnet run
```

### Release Build

```
dotnet build -c Release
```

The output executable is in bin/Release/net9.0-windows/

### Dependencies

- .NET 9.0 Windows Desktop Runtime
- Newtonsoft.Json (NuGet package)
- Windows Forms

## Testing Guidelines

Manual testing checklist:

- Timer countdown accuracy (use Test Break button)
- Settings persistence (close and reopen app)
- Break screen display and animations
- Keyboard shortcuts (Space, Enter, Esc)
- Sound playback (if enabled)
- System tray functionality
- Statistics tracking
- Theme switching
- Configuration file creation in AppData

## Extending the Application

### Adding New Health Messages

Edit BreakMessages.cs:

1. Add new message array
2. Add category to message rotation logic
3. Messages display automatically

### Adding Configuration Options

1. Add property to AppConfig.cs
2. Add JSON serialization in ConfigManager.cs
3. Add UI control in SettingsForm.cs
4. Update LoadSettingsToForm() and SaveSettingsFromForm() methods

### Implementing Custom Sounds

Use SoundManager.cs with .wav files:

```csharp
SoundManager.PlayCustomSound("path/to/sound.wav");
```

### Modifying Themes

Update AppConfig.cs color properties and apply in BreakForm.cs ApplyTheme() method.

## Code Style Guidelines

- Follow C# naming conventions (PascalCase for public, camelCase for private)
- Use meaningful variable and method names
- Add XML documentation comments to public members
- Keep methods focused and under 50 lines when possible
- Use guard clauses for null checks
- Organize code using regions (#region / #endregion)

## Performance Considerations

- Timer ticks use minimal CPU when break screen not displayed
- JSON files are small (typically under 10 KB)
- No database queries or network calls
- Memory usage minimal (under 50 MB)
- Disk writes occur only during configuration changes or breaks

## Troubleshooting Build Issues

### Missing Dependencies

```
dotnet restore
```

### Newtonsoft.Json Not Found

```
dotnet add package Newtonsoft.Json
```

### Windows Runtime Issues

Ensure .NET 9.0 Windows Desktop Runtime is installed.

### Compilation Errors

Check that all files are in the project directory and listed in BreakTimer.csproj.

## Version History

### v2.0
- Professional layered architecture
- Statistics tracking
- Configuration persistence
- Break screen animations
- Keyboard shortcuts

### v1.0
- Basic timer functionality
- Simple break screen
- System tray integration
