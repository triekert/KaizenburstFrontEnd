<#
.SYNOPSIS
    Finds and processes all Visual Studio solution files, cleaning, wiping bin/obj folders, and rebuilding each.
.DESCRIPTION
    This script shuts down lingering build servers, searches for all .sln files recursively, 
    loops through each one, executes 'dotnet clean', forcefully removes local bin and obj folders, 
    and runs a non-incremental 'dotnet build' for each solution found.
#>

[CmdletBinding()]
param(
    [Parameter(HelpMessage="The build configuration, e.g., Debug or Release")]
    [ValidateSet("Debug", "Release")]
    [string]$Configuration = "Debug"
)

# Stop execution on errors
$ErrorActionPreference = "Stop"

# Step 0: Clear any stuck build servers to prevent timeout warnings
Write-Host "Step 0: Clearing lingering MSBuild/dotnet servers..." -ForegroundColor Yellow
dotnet build-server shutdown
Stop-Process -Name "MSBuild" -ErrorAction SilentlyContinue

Write-Host "Searching for all Visual Studio solution (.sln) files..." -ForegroundColor Cyan
$slnFiles = Get-ChildItem -Filter "*.sln" -Recurse

if (-not $slnFiles) {
    Write-Error "No .sln files found in the current directory or its subdirectories."
    exit 1
}

Write-Host "Found $($slnFiles.Count) solution file(s). Starting batch clean and rebuild..." -ForegroundColor Green

foreach ($slnFile in $slnFiles) {
    Write-Host "`n----------------------------------------" -ForegroundColor Cyan
    Write-Host "Processing Solution: $($slnFile.Name)" -ForegroundColor Cyan
    Write-Host "Path: $($slnFile.FullName)" -ForegroundColor DarkCyan
    Write-Host "----------------------------------------" -ForegroundColor Cyan

    $solutionPath = $slnFile.FullName
    $solutionDir = $slnFile.DirectoryName

    # Step 1: Run dotnet clean
    Write-Host "Step 1: Running 'dotnet clean'..." -ForegroundColor Yellow
    dotnet clean "$solutionPath" --configuration $Configuration

    # Step 2: Wipe bin and obj folders for this solution's directory
    Write-Host "Step 2: Wiping all 'bin' and 'obj' folders..." -ForegroundColor Yellow
    $foldersToWipe = Get-ChildItem -Path $solutionDir -Recurse -Directory -Include "bin", "obj"
    if ($foldersToWipe) {
        $foldersToWipe | Remove-Item -Recurse -Force
        Write-Host "Successfully removed $($foldersToWipe.Count) build artifact directories." -ForegroundColor Green
    } else {
        Write-Host "No bin or obj folders found to remove." -ForegroundColor DarkYellow
    }

    # Step 3: Rebuild the solution
    Write-Host "Step 3: Rebuilding solution ($Configuration)..." -ForegroundColor Yellow
    dotnet build "$solutionPath" --configuration $Configuration --no-incremental

    if ($LASTEXITCODE -eq 0) {
        Write-Host "Success! Solution '$($slnFile.Name)' has been completely cleaned and rebuilt." -ForegroundColor Green
    } else {
        Write-Error "Build failed for '$($slnFile.Name)' with exit code $LASTEXITCODE."
        exit $LASTEXITCODE
    }
}

Write-Host "`nAll solutions successfully cleaned and rebuilt!" -ForegroundColor Green