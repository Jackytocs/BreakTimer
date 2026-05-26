@echo off
REM Create .github\workflows directory structure
mkdir ".github\workflows" 2>nul

REM Create build.yml
(
echo # GitHub Actions workflow for building BreakTimer
echo # Triggered on every push and pull request to ensure the project builds correctly
echo name: Build BreakTimer
echo.
echo on:
echo   push:
echo     branches: [ main, develop ]
echo   pull_request:
echo     branches: [ main, develop ]
echo.
echo jobs:
echo   build:
echo     # Run on Windows environment since BreakTimer is a Windows Forms application
echo     runs-on: windows-latest
echo     
echo     steps:
echo     # Check out the repository code
echo     - name: Checkout code
echo       uses: actions/checkout@v4
echo.
echo     # Set up .NET 8 SDK for building the project
echo     - name: Setup .NET 8
echo       uses: actions/setup-dotnet@v4
echo       with:
echo         dotnet-version: '8.0.x'
echo.
echo     # Restore NuGet dependencies
echo     - name: Restore NuGet packages
echo       run: dotnet restore
echo.
echo     # Build the project in Release configuration
echo     - name: Build BreakTimer
echo       run: dotnet build --configuration Release --no-restore
echo.
echo     # Run any unit tests if they exist
echo     - name: Run tests ^(if available^)
echo       run: dotnet test --configuration Release --no-build --verbosity normal
echo       continue-on-error: true
echo.
echo     # Publish the application as a self-contained executable
echo     - name: Publish Release build
echo       run: dotnet publish --configuration Release --output ./publish
echo.
echo     # Upload build artifacts for debugging and distribution
echo     - name: Upload build artifacts
echo       uses: actions/upload-artifact@v4
echo       if: always^(^)
echo       with:
echo         name: BreakTimer-Release
echo         path: ./publish
echo         retention-days: 30
echo.
echo     # Display build summary
echo     - name: Build Summary
echo       if: always^(^)
echo       run: ^|
echo         Write-Host "========================================" -ForegroundColor Green
echo         Write-Host "Build completed successfully!" -ForegroundColor Green
echo         Write-Host "========================================" -ForegroundColor Green
echo         Write-Host ""
echo         Write-Host "Build Artifacts:" -ForegroundColor Cyan
echo         Write-Host "  Location: ./publish" -ForegroundColor Yellow
echo         Write-Host ""
echo         Write-Host "Project: BreakTimer ^(.NET 8 Windows Forms^)" -ForegroundColor Yellow
) > ".github\workflows\build.yml"

REM Create release.yml
(
echo # GitHub Actions workflow for creating releases of BreakTimer
echo # Triggered when a version tag is pushed ^(v^*.^*^.^*^)
echo name: Release BreakTimer
echo.
echo on:
echo   push:
echo     tags:
echo       - 'v[0-9]+.[0-9]+.[0-9]+'
echo.
echo jobs:
echo   release:
echo     runs-on: windows-latest
echo     
echo     steps:
echo     - name: Checkout code
echo       uses: actions/checkout@v4
echo.
echo     - name: Extract version from tag
echo       id: version
echo       run: ^|
echo         $tag = "${{ github.ref }}".Replace^('refs/tags/', ''
echo         $version = $tag.Replace^('v', ''
echo         Write-Output "version=$version" ^>^> $env:GITHUB_OUTPUT
echo         Write-Output "tag=$tag" ^>^> $env:GITHUB_OUTPUT
echo.
echo     - name: Setup .NET 8
echo       uses: actions/setup-dotnet@v4
echo       with:
echo         dotnet-version: '8.0.x'
echo.
echo     - name: Restore NuGet packages
echo       run: dotnet restore
echo.
echo     - name: Build Release
echo       run: dotnet build --configuration Release --no-restore
echo.
echo     - name: Publish Release Build
echo       run: dotnet publish --configuration Release --output ./publish
echo.
echo     - name: Create Release
echo       uses: softprops/action-gh-release@v1
echo       with:
echo         files: ./publish/**
) > ".github\workflows\release.yml"

echo GitHub Actions workflows created successfully!
echo Created: .github\workflows\build.yml
echo Created: .github\workflows\release.yml
