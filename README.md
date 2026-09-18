# The Dark Sun Chronicles - DMG Companion

A native, offline Windows 11 Game Master application for running campaigns across the Seven Worlds.

## Startup bug reports

The published application creates `DSC.Toolkit-bug-report.txt` in the same folder as `DSC.Toolkit.exe` as soon as it starts. It records the operating system, process architecture, application files, and the last startup stage reached. If the program crashes before a window appears, the report still contains the last successful startup step.

To report a startup failure:

1. Extract the complete x64 ZIP into a writable folder.
2. Run `DSC.Toolkit.exe` once.
3. Send `DSC.Toolkit-bug-report.txt` together with the exact Windows Event Viewer error.

The application never sends telemetry. Diagnostic reports are local text files and may contain local file names, so review them before sharing.

## Requirements

- Windows 11, build 22000 or later.
- Visual Studio 2022 with **.NET desktop development** and **Windows application development** workloads, or the .NET 8 SDK plus the required Windows SDK components.

The project publishes self-contained Windows App SDK files. The Visual C++ 2015-2022 x64 Redistributable is also required for an unpackaged x64 WinUI application.

## Build locally

```powershell
git clone <your-repository-url> C:\darksun
cd C:\darksun
dotnet restore .\src\DSC.Toolkit\DSC.Toolkit.csproj -r win-x64
dotnet test .\tests\DSC.Toolkit.Tests\DSC.Toolkit.Tests.csproj -c Release
dotnet publish .\src\DSC.Toolkit\DSC.Toolkit.csproj -c Release -r win-x64 --self-contained true -o .\artifacts\win-x64
```

Run `artifacts\win-x64\DSC.Toolkit.exe`.
