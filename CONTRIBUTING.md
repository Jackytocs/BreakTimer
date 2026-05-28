# Contributing to BreakTimer

Thank you for your interest in contributing to BreakTimer! This document provides guidelines and instructions for contributing.

## Code of Conduct

We are committed to providing a welcoming and inclusive environment. Please:

- Be respectful to all contributors
- Provide constructive feedback
- Focus on the code, not the person
- Help others succeed

## Getting Started

### Development Setup

**Prerequisites:**

- Windows 10/11 or WSL2 with Windows support
- .NET 8 SDK (or later)
- Git
- Visual Studio 2022 / VS Code (optional but recommended)

**Steps:**

```bash
git clone https://github.com/Jackytocs/BreakTimer.git
cd BreakTimer
dotnet build
dotnet run
```

### Project Structure

```
BreakTimer/
├── Program.cs           # Entry point, exception handling
├── SettingsForm.cs      # Configuration UI, timer engine
├── BreakForm.cs         # Break screen, multi-monitor support
├── ConfigManager.cs     # Settings/stats persistence
├── SessionStatistics.cs # Break tracking
├── AppConfig.cs         # Configuration model
├── SoundManager.cs      # Audio notifications
├── BreakMessages.cs     # Health tips
└── BreakTimer.csproj    # Project file
```

## Issues

### Reporting Bugs

When reporting a bug, please include:

1. **Title**: Clear, descriptive summary
2. **Environment**:
   - Windows version (10/11, build number)
   - .NET runtime version (`dotnet --version`)
   - Display setup (single/multi-monitor)
3. **Reproduction Steps**: Detailed steps to reproduce
4. **Expected Behavior**: What should happen
5. **Actual Behavior**: What actually happened
6. **Screenshots/Videos**: If applicable
7. **Error Logs**: Contents of `%APPDATA%\BreakTimer\crash_log.txt`

### Suggesting Features

When suggesting features:

1. **Title**: Clear feature name
2. **Motivation**: Why this feature is needed
3. **Use Case**: Specific scenario where it helps
4. **Implementation Notes**: Any ideas on how to implement
5. **Alternatives Considered**: Other possible approaches

## Pull Requests

### Before You Start

1. Check existing issues to avoid duplicates
2. Discuss major changes in an issue first
3. Fork the repository
4. Create a feature branch: `git checkout -b feature/your-feature-name`

### Code Style

Follow these conventions:

**C# Naming:**

```csharp
public class MyClassName { }           // PascalCase for classes
public void MyMethodName() { }          // PascalCase for methods
private string myFieldName;             // camelCase for fields
private const int MyConstant = 42;      // PascalCase for constants
```

**Formatting:**

```csharp
// Use 4 spaces for indentation
if (condition)
{
    DoSomething();
}

// Add XML documentation for public members
/// <summary>
/// Describes what this method does.
/// </summary>
/// <param name="parameter">Description of parameter</param>
/// <returns>Description of return value</returns>
public string MyMethod(int parameter)
{
    return "result";
}
```

**Best Practices:**

- Enable nullable reference types: `#nullable enable`
- Use `using` statements for resource disposal
- Use `async/await` for long operations, not `Thread.Sleep()`
- Check for null and validate inputs
- Keep methods focused and small
- Add comments for complex logic only
- Follow DRY (Don't Repeat Yourself)

### Commit Messages

Use clear, descriptive commit messages:

```
Fix: Correct UI thread blocking issue in BreakForm
Refactor: Simplify timer logic with DateTime-based tracking
Feature: Add multi-monitor support
Docs: Update README with new features
```

Format: `[Type]: Brief description`

Valid types:

- `feat` - New feature
- `fix` - Bug fix
- `refactor` - Code restructuring
- `docs` - Documentation changes
- `style` - Formatting/style changes
- `perf` - Performance improvements
- `test` - Adding/updating tests
- `chore` - Build/dependency changes

### Testing

Before submitting:

1. **Build**: `dotnet build -c Release`
2. **Run Application**: Manually test the functionality
3. **Test Scenarios**:
   - Single-monitor setup
   - Multi-monitor setup (if possible)
   - Normal timer completion
   - Skip button functionality
   - Settings save/load
   - System tray interactions

### Creating the Pull Request

1. Push your branch: `git push origin feature/your-feature-name`
2. Create Pull Request on GitHub
3. Fill out the PR template with:
   - Description of changes
   - Motivation and context
   - Testing performed
   - Screenshots (if UI changes)
   - Checklist verification

**PR Checklist:**

- [ ] Code follows style guidelines
- [ ] Self-reviewed your own code
- [ ] Added comments for complex logic
- [ ] Documentation updated if needed
- [ ] No new warnings/errors
- [ ] Tested on Windows 10/11
- [ ] Changes don't break existing functionality

## Branching Strategy

- `main` - Stable, release-ready code
- `develop` - Integration branch for features
- `feature/*` - Feature branches
- `fix/*` - Bug fix branches
- `docs/*` - Documentation updates

### Branch Naming

```
feature/add-system-lock-detection
fix/timer-drift-on-suspend
docs/update-readme-setup
refactor/simplify-timer-logic
```

## Release Process

Releases follow semantic versioning: `MAJOR.MINOR.PATCH`

- `MAJOR`: Breaking changes
- `MINOR`: New features (backward compatible)
- `PATCH`: Bug fixes

### For Maintainers Only

1. Update version in `BreakTimer.csproj`
2. Update `CHANGELOG.md`
3. Create git tag: `git tag v1.2.3`
4. Push tag: `git push origin v1.2.3`
5. Create GitHub Release with notes

## Documentation

### README Updates

- Reflect new features/changes
- Keep installation steps current
- Update feature list
- Add screenshots for UI changes

### Code Comments

Only comment complex logic:

- **Good**: Explains *why*, not what
- **Bad**: States what the code does (already clear)

```csharp
// Good - explains reasoning
// Use DateTime delta to handle system sleep/suspend
TimeSpan timeLeft = workEndTime.Value - DateTime.Now;

// Bad - obvious from code
// Set x to 5
int x = 5;
```

### XML Documentation

Add for public APIs:

```csharp
/// <summary>
/// Saves configuration atomically to prevent corruption on system failure.
/// </summary>
/// <param name="filePath">Full path to configuration file</param>
/// <param name="content">JSON content to persist</param>
public void SaveAtomically(string filePath, string content)
{
    // Implementation
}
```

## Performance Considerations

- Avoid `Thread.Sleep()` - use `async/await` instead
- Dispose timers and resources properly
- Use `DateTime.Now` for precision instead of ticks
- Minimize allocations in timer ticks
- Profile before optimizing

## Accessibility

- Ensure UI is keyboard navigable
- Use proper color contrast (WCAG AA minimum)
- Test with high DPI displays
- Support system theme changes

## Security

- Never log sensitive data
- Validate all user inputs
- Use proper registry path quoting
- Implement atomic file operations
- Check permissions before registry writes

## Licensing

By contributing, you agree that your contributions will be licensed under the same MIT License as the project.

---

## Review Process

1. Automated checks (build, linting)
2. Code review by maintainers
3. Feedback and iterations
4. Approval and merge

### Review Feedback

- Be receptive to suggestions
- Ask for clarification if needed
- Update code based on feedback
- Re-request review after changes

## Questions?

- Open a GitHub Discussion for questions
- Check existing issues for answers
- Be patient - volunteers respond when available

## Recognition

Contributors will be recognized in:

- README contributors section
- Release notes for PR authors
- GitHub contributors page

---

Thank you for helping make BreakTimer better! 🙏

**Remember**: Regular contributions improve the project, but quality matters more than quantity.
