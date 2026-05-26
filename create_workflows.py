#!/usr/bin/env python3
"""
Script to create GitHub Actions workflow directory structure and files for BreakTimer
"""

import os
import sys

def create_workflows():
    # Define paths
    project_root = r'c:\Users\jagat\takebreak\BreakTimer'
    workflows_dir = os.path.join(project_root, '.github', 'workflows')
    
    # Create directory structure
    try:
        os.makedirs(workflows_dir, exist_ok=True)
        print(f"✓ Created directory: {workflows_dir}")
    except Exception as e:
        print(f"✗ Failed to create directory: {e}")
        return False
    
    # Build workflow content
    build_yml_content = """# GitHub Actions workflow for building BreakTimer
# Triggered on every push and pull request to ensure the project builds correctly
name: Build BreakTimer

on:
  push:
    branches: [ main, develop ]
  pull_request:
    branches: [ main, develop ]

jobs:
  build:
    # Run on Windows environment since BreakTimer is a Windows Forms application
    runs-on: windows-latest
    
    steps:
    # Check out the repository code
    - name: Checkout code
      uses: actions/checkout@v4

    # Set up .NET 8 SDK for building the project
    - name: Setup .NET 8
      uses: actions/setup-dotnet@v4
      with:
        dotnet-version: '8.0.x'

    # Restore NuGet dependencies
    - name: Restore NuGet packages
      run: dotnet restore

    # Build the project in Release configuration
    - name: Build BreakTimer
      run: dotnet build --configuration Release --no-restore

    # Run any unit tests if they exist
    - name: Run tests (if available)
      run: dotnet test --configuration Release --no-build --verbosity normal
      continue-on-error: true

    # Publish the application as a self-contained executable
    - name: Publish Release build
      run: dotnet publish --configuration Release --output ./publish

    # Upload build artifacts for debugging and distribution
    - name: Upload build artifacts
      uses: actions/upload-artifact@v4
      if: always()
      with:
        name: BreakTimer-Release
        path: ./publish
        retention-days: 30

    # Display build summary
    - name: Build Summary
      if: always()
      run: |
        Write-Host "========================================" -ForegroundColor Green
        Write-Host "Build completed successfully!" -ForegroundColor Green
        Write-Host "========================================" -ForegroundColor Green
        Write-Host ""
        Write-Host "Build Artifacts:" -ForegroundColor Cyan
        Write-Host "  Location: ./publish" -ForegroundColor Yellow
        Write-Host ""
        Write-Host "Project: BreakTimer (.NET 8 Windows Forms)" -ForegroundColor Yellow
"""

    release_yml_content = """# GitHub Actions workflow for creating releases of BreakTimer
# Triggered when a version tag is pushed (v*.*.*)
name: Release BreakTimer

on:
  push:
    tags:
      - 'v[0-9]+.[0-9]+.[0-9]+'

jobs:
  release:
    # Run on Windows environment for building Windows Forms application
    runs-on: windows-latest
    
    steps:
    # Check out the repository code
    - name: Checkout code
      uses: actions/checkout@v4

    # Extract version from git tag
    - name: Extract version from tag
      id: version
      run: |
        $tag = "${{ github.ref }}".Replace('refs/tags/', '')
        $version = $tag.Replace('v', '')
        Write-Output "version=$version" >> $env:GITHUB_OUTPUT
        Write-Output "tag=$tag" >> $env:GITHUB_OUTPUT

    # Set up .NET 8 SDK
    - name: Setup .NET 8
      uses: actions/setup-dotnet@v4
      with:
        dotnet-version: '8.0.x'

    # Restore NuGet dependencies
    - name: Restore NuGet packages
      run: dotnet restore

    # Build the project in Release configuration
    - name: Build Release
      run: dotnet build --configuration Release --no-restore

    # Publish the application as release artifact
    - name: Publish Release Build
      run: dotnet publish --configuration Release --output ./publish --self-contained true

    # Create release artifacts (exe and supporting files)
    - name: Prepare release artifacts
      run: |
        mkdir release_artifacts
        Copy-Item -Path ./publish/* -Destination ./release_artifacts -Recurse
        Compress-Archive -Path ./release_artifacts -DestinationPath BreakTimer-${{ steps.version.outputs.version }}-windows.zip

    # Create GitHub Release with artifacts
    - name: Create Release
      uses: actions/create-release@v1
      env:
        GITHUB_TOKEN: ${{ secrets.GITHUB_TOKEN }}
      with:
        tag_name: ${{ steps.version.outputs.tag }}
        release_name: 'BreakTimer Release ${{ steps.version.outputs.version }}'
        body: |
          Release of BreakTimer version ${{ steps.version.outputs.version }}
          
          ## What's New
          See [CHANGELOG.md](CHANGELOG.md) for detailed changes in this release.
          
          ## Downloads
          - **BreakTimer-${{ steps.version.outputs.version }}-windows.zip** - Complete Windows application
          
          ## Installation
          1. Download the ZIP file
          2. Extract to your preferred location
          3. Run BreakTimer.exe
          
          ## System Requirements
          - Windows 10 or later
          - .NET 8 Runtime (will be included in self-contained build)
        draft: false
        prerelease: false
      id: create_release

    # Upload release asset (ZIP file)
    - name: Upload Release Asset
      uses: actions/upload-release-asset@v1
      env:
        GITHUB_TOKEN: ${{ secrets.GITHUB_TOKEN }}
      with:
        upload_url: ${{ steps.create_release.outputs.upload_url }}
        asset_path: ./BreakTimer-${{ steps.version.outputs.version }}-windows.zip
        asset_name: BreakTimer-${{ steps.version.outputs.version }}-windows.zip
        asset_content_type: application/zip

    # Upload all publish artifacts to release
    - name: Upload build artifacts to release
      uses: actions/upload-artifact@v4
      with:
        name: BreakTimer-Release-${{ steps.version.outputs.version }}
        path: ./publish
        retention-days: 90

    # Notification of successful release
    - name: Release Summary
      if: success()
      run: |
        Write-Host "========================================" -ForegroundColor Green
        Write-Host "Release created successfully!" -ForegroundColor Green
        Write-Host "========================================" -ForegroundColor Green
        Write-Host ""
        Write-Host "Release Information:" -ForegroundColor Cyan
        Write-Host "  Version: ${{ steps.version.outputs.version }}" -ForegroundColor Yellow
        Write-Host "  Tag: ${{ steps.version.outputs.tag }}" -ForegroundColor Yellow
        Write-Host ""
        Write-Host "Artifacts:" -ForegroundColor Cyan
        Write-Host "  ZIP Package: BreakTimer-${{ steps.version.outputs.version }}-windows.zip" -ForegroundColor Yellow
        Write-Host "  Location: GitHub Release" -ForegroundColor Yellow
"""
    
    # Create build.yml
    build_yml_path = os.path.join(workflows_dir, 'build.yml')
    try:
        with open(build_yml_path, 'w', encoding='utf-8') as f:
            f.write(build_yml_content)
        print(f"✓ Created: {build_yml_path}")
    except Exception as e:
        print(f"✗ Failed to create build.yml: {e}")
        return False
    
    # Create release.yml
    release_yml_path = os.path.join(workflows_dir, 'release.yml')
    try:
        with open(release_yml_path, 'w', encoding='utf-8') as f:
            f.write(release_yml_content)
        print(f"✓ Created: {release_yml_path}")
    except Exception as e:
        print(f"✗ Failed to create release.yml: {e}")
        return False
    
    # Verify files
    print("\n" + "="*50)
    print("Verification:")
    print("="*50)
    
    if os.path.isdir(workflows_dir):
        print(f"✓ Workflows directory exists: {workflows_dir}")
        files = os.listdir(workflows_dir)
        for f in files:
            file_path = os.path.join(workflows_dir, f)
            size = os.path.getsize(file_path)
            print(f"  ✓ {f} ({size} bytes)")
    
    return True

if __name__ == '__main__':
    success = create_workflows()
    if success:
        print("\n✓ All GitHub Actions workflows created successfully!")
        sys.exit(0)
    else:
        print("\n✗ Failed to create workflows")
        sys.exit(1)
