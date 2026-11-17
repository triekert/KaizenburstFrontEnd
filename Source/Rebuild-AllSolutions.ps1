<#
.SYNOPSIS
    Finds and processes all Visual Studio solution files, wiping bin/obj folders and rebuilding each.
.DESCRIPTION
    Terminates background compilers, safely removes bin and obj folders with graceful error handling 
    for locked files, and runs a non-incremental 'dotnet build'.
#>

[CmdletBinding()]
param(
    [Parameter(HelpMessage="The build configuration, e.g., Debug or Release")]
    [ValidateSet("Debug", "Release")]
    [string]$Configuration = "Debug"
)

# Keep error preference flexible for folder cleanups
$ErrorActionPreference = "Continue"

# Step 0: Aggressively stop running apps, build servers, and background compilers
Write-Host "Step 0: Stopping application instances, build servers, and compilers..." -ForegroundColor Yellow
Stop-Process -Name "Fasetto.Word" -ErrorAction SilentlyContinue
Stop-Process -Name "VBCSCompiler" -ErrorAction SilentlyContinue
Stop-Process -Name "MSBuild" -ErrorAction SilentlyContinue
Stop-Process -Name "dotnet" -ErrorAction SilentlyContinue
dotnet build-server shutdown

# Give Windows a pause to fully release file handles
Write-Host "Waiting for Windows to release file system locks..." -ForegroundColor DarkYellow
Start-Sleep -Seconds 3

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

    # Step 1: Wipe bin and obj folders safely without aborting on file locks
    Write-Host "Step 1: Wiping all 'bin' and 'obj' folders..." -ForegroundColor Yellow
    $foldersToWipe = Get-ChildItem -Path $solutionDir -Recurse -Directory -Include "bin", "obj"
    
    if ($foldersToWlp -ne $null -or $foldersToWipe.Count -gt 0) {
        foreach ($folder in $foldersToWipe) {
            $folderPath = $folder.FullName
            if (Test-Path $folderPath) {
                $maxRetries = 3
                $deleted = $false

                for ($i = 1; $i -le $maxRetries; $i++) {
                    # Attempt removal using .NET IO instead of cmd.exe for cleaner error handling
                    try {
                        [System.IO.Directory]::Delete($folderPath, $true)
                        $deleted = $true
                        break
                    } catch {
                        Start-Sleep -Seconds 1
                    }
                }

                if ($deleted) {
                    Write-Host "  Successfully removed: $(Split-Path $folderPath -Leaf)" -ForegroundColor DarkGreen
                } else {
                    Write-Warning "  Could not fully remove locked folder: $folderPath (Proceeding with build anyway)"
                }
            }
        }
    } else {
        Write-Host "No bin or obj folders found to remove." -ForegroundColor DarkYellow
    }

    # Step 2: Rebuild the solution fresh
    Write-Host "Step 2: Rebuilding solution ($Configuration)..." -ForegroundColor Yellow
    dotnet build "$solutionPath" --configuration $Configuration --no-incremental

    if ($LASTEXITCODE -eq 0) {
        Write-Host "Success! Solution '$($slnFile.Name)' has been completely cleaned and rebuilt." -ForegroundColor Green
    } else {
        Write-Error "Build failed for '$($slnFile.Name)' with exit code $LASTEXITCODE."
        exit $LASTEXITCODE
    }
}

Write-Host "`nAll solutions successfully processed!" -ForegroundColor Green