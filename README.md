# The Dark Sun Chronicles - DMG Companion

A native, offline Windows 11 Game Master application for running campaigns across the Seven Worlds. The interface uses the planets as seven thematic tool domains and converts the Dungeon Master's Guide worksheets into editable campaign records rather than acting as a PDF reader.

## Current capabilities

- Exactly seven planetary Shard modules.
- DMG Chapter 12 worksheet templates with source page references.
- Campaign records, stable identifiers, tags, links, global search, atomic saves, imports, and rolling backups.
- Generic tracks for corruption, resonance, exposure, instability, faction pressure, or any custom state.
- Safe dice expressions (`1d20+5`, `2d6-3`, `1d100`), d4 through d100, modifiers, advantage/disadvantage, labels, and session history.
- Canonical Seven Locks seed data.
- Dark emerald, obsidian, antique-gold, ivory, and planet-accent visual system.
- No telemetry, accounts, cloud dependency, or runtime PDF requirement.

## Requirements

- Windows 11, build 22000 or later.
- Visual Studio 2022 with **.NET desktop development** and **Windows application development** workloads, or the .NET 8 SDK plus the required Windows SDK components.

The project pins the stable `Microsoft.WindowsAppSDK` 2.4.0 package. The application is published self-contained, so end users do not need to install the Windows App SDK runtime separately.

## Build locally

```powershell
git clone <your-repository-url> C:\darksun
cd C:\darksun
dotnet restore .\src\DSC.Toolkit\DSC.Toolkit.csproj -r win-x64
dotnet test .\tests\DSC.Toolkit.Tests\DSC.Toolkit.Tests.csproj -c Release
dotnet publish .\src\DSC.Toolkit\DSC.Toolkit.csproj -c Release -r win-x64 --self-contained true -o .\artifacts\win-x64
```

Run `artifacts\win-x64\DSC.Toolkit.exe`.

## GitHub build and releases

Push the repository to GitHub. Every push to `main` or `master` runs core tests, portability checks, and separate Windows x64/ARM64 publishes. Download builds from the workflow's **Artifacts** section.

To create a release:

```powershell
git tag v0.1.0
git push origin v0.1.0
```

The tagged-release workflow tests the project, creates architecture-specific ZIP files, and attaches them to the GitHub Release. GitHub Pages is not used because this is a native desktop application.

## Canonical manual

The source DMG must remain local and must not be committed. It was used to verify operational field names and source pages, and is excluded by `.gitignore`. The app contains concise worksheet field definitions and references, not the full manual.

## Campaign data

Campaigns are stored beneath `%LOCALAPPDATA%\DarkSunChronicles\DMGCompanion`. Save files are human-readable, versioned JSON. Writes use a temporary file and replacement; previous versions rotate through the `Backups` folder. Importing creates a new campaign identity unless replacement is explicitly requested.

## Architecture

- `DSC.Toolkit.Core`: platform-independent domain models, dice, trackers, persistence, and search.
- `DSC.Toolkit`: WinUI 3 XAML views and MVVM presentation logic.
- `DSC.Toolkit.Tests`: tests that can run independently of WinUI.
- `Data/Rules`: compact source manifest and future setting rule packs.

The seven modules share models and controls. Planet-specific behavior is data-driven to avoid duplicated pages and fragile code.

## Copyright and distribution

The Dark Sun Chronicles setting and supplied manual remain the owner's material. See `LICENSE` before public distribution. Third-party package notices are listed in `THIRD_PARTY_NOTICES.md`.
