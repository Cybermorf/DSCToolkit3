using System.Reflection;
using Microsoft.UI.Xaml;

namespace DSC.Toolkit;

public partial class App : Application
{
    private Window? _window;

    public App()
    {
        StartupDiagnostics.Begin();

        try
        {
            // Required for unpackaged WinUI 3 / Windows App SDK apps.
            // This must happen before InitializeComponent() creates any WinUI XAML objects.
            Microsoft.WindowsAppRuntime.Bootstrapper.Initialize();

            InitializeComponent();
            UnhandledException += OnUnhandledException;
            AppDomain.CurrentDomain.UnhandledException += OnDomainUnhandledException;
            TaskScheduler.UnobservedTaskException += OnUnobservedTaskException;
            StartupDiagnostics.Write("Application resources initialized.");
        }
        catch (Exception exception)
        {
            StartupDiagnostics.WriteException("App constructor failed", exception);
            throw;
        }
    }

    protected override void OnLaunched(LaunchActivatedEventArgs args)
    {
        try
        {
            StartupDiagnostics.Write("Creating MainWindow.");
            _window = new MainWindow();
            StartupDiagnostics.Write("MainWindow created; activating window.");
            _window.Activate();
            StartupDiagnostics.Write("Window activated successfully.");
        }
        catch (Exception exception)
        {
            StartupDiagnostics.WriteException("Application launch failed", exception);
            throw;
        }
    }

    private static void OnUnhandledException(object sender, Microsoft.UI.Xaml.UnhandledExceptionEventArgs args)
    {
        StartupDiagnostics.WriteException("WinUI unhandled exception", args.Exception);
        args.Handled = false;
    }

    private static void OnDomainUnhandledException(object? sender, System.UnhandledExceptionEventArgs args)
    {
        if (args.ExceptionObject is Exception exception)
            StartupDiagnostics.WriteException("AppDomain unhandled exception", exception);
        else
            StartupDiagnostics.Write($"AppDomain unhandled exception: {args.ExceptionObject}");
    }

    private static void OnUnobservedTaskException(object? sender, UnobservedTaskExceptionEventArgs args)
    {
        StartupDiagnostics.WriteException("Unobserved task exception", args.Exception);
    }
}

internal static class StartupDiagnostics
{
    private static readonly object Sync = new();
    private static string? _reportPath;

    public static string ReportPath => _reportPath ??= Path.Combine(AppContext.BaseDirectory, "DSC.Toolkit-bug-report.txt");

    public static void Begin()
    {
        try
        {
            var report = ReportPath;
            lock (Sync)
            {
                File.WriteAllText(report, $"DSC Toolkit startup diagnostic report{Environment.NewLine}Generated: {DateTimeOffset.Now:O}{Environment.NewLine}{Environment.NewLine}");
                File.AppendAllText(report, EnvironmentReport());
                File.AppendAllText(report, $"Executable: {Environment.ProcessPath}{Environment.NewLine}Working directory: {Environment.CurrentDirectory}{Environment.NewLine}{Environment.NewLine}");
            }
        }
        catch
        {
        }
    }

    public static void Write(string message)
    {
        try
        {
            lock (Sync)
                File.AppendAllText(ReportPath, $"[{DateTimeOffset.Now:O}] {message}{Environment.NewLine}");
        }
        catch
        {
        }
    }

    public static void WriteException(string stage, Exception exception) => Write($"{stage}:{Environment.NewLine}{exception}{Environment.NewLine}");

    private static string EnvironmentReport()
    {
        var assembly = Assembly.GetEntryAssembly();
        var lines = new List<string>
        {
            $"OS: {Environment.OSVersion}",
            $"64-bit OS: {Environment.Is64BitOperatingSystem}",
            $"64-bit process: {Environment.Is64BitProcess}",
            $"Process architecture: {System.Runtime.InteropServices.RuntimeInformation.ProcessArchitecture}",
            $".NET: {System.Runtime.InteropServices.RuntimeInformation.FrameworkDescription}",
            $"App version: {assembly?.GetName().Version}",
            "",
            "Files in application folder:"
        };

        try
        {
            foreach (var file in Directory.EnumerateFiles(AppContext.BaseDirectory, "*", SearchOption.TopDirectoryOnly).OrderBy(Path.GetFileName))
            {
                var info = new FileInfo(file);
                lines.Add($"  {info.Name} ({info.Length} bytes)");
            }
        }
        catch (Exception exception)
        {
            lines.Add($"  Unable to list files: {exception.Message}");
        }

        lines.Add("");
        return string.Join(Environment.NewLine, lines) + Environment.NewLine;
    }
}
