# Implementation plan

1. Keep all rules, dice, tracks, persistence, search, and links in a platform-independent core.
2. Use one XAML/MVVM shell and one shared record editor for all seven planet modules.
3. Represent DMG worksheets as compact definitions with source-page references.
4. Store mutable campaigns as versioned JSON under LocalAppData using atomic writes and rotating backups.
5. Compile and test WinUI on GitHub's Windows runners; publish self-contained x64 and ARM64 artifacts.
6. Iterate from real Windows 11 use: improve form types, pickers, relationship visualization, accessibility, and visual polish without duplicating modules.

The uploaded manual is a development reference only and is neither committed nor required at runtime.
