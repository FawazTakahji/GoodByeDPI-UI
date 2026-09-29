using GoodByeDPI.Core.Dialogs;
using GoodByeDPI.Core.Packages;
using GoodByeDPI.Core.Services;
using GoodByeDPI.Core.Theming;
using Microsoft.Extensions.Logging;

namespace GoodByeDPI.Core.Processes;

public class GoodByeDpiService
{
    private readonly ILogger<GoodByeDpiService> _logger;
    private readonly PackageManager _packages;
    private readonly ProcessManager _process;
    private readonly IDialogService _dialogs;
    private readonly IToastService _toasts;
    private readonly IThemeService _theme;
    private readonly IWindowService _window;
    private readonly ITrayNotifyService _trayNotify;

    private string? _resolvedExePath;

    public bool IsRunning => _process.IsRunning;
    public bool IsBusy { get; private set; }

    public event EventHandler<GoodByeDpiStateChangedEventArgs>? StateChanged;

    public GoodByeDpiService(
        ILogger<GoodByeDpiService> logger,
        PackageManager packages,
        ProcessManager process,
        IDialogService dialogs,
        IToastService toasts,
        IThemeService theme,
        IWindowService window,
        ITrayNotifyService trayNotify)
    {
        _logger = logger;
        _packages = packages;
        _process = process;
        _dialogs = dialogs;
        _toasts = toasts;
        _theme = theme;
        _window = window;
        _trayNotify = trayNotify;

        _process.StateChanged += OnProcessStateChanged;
    }

    public async Task ToggleAsync()
    {
        if (IsRunning)
        {
            await StopAsync();
        }
        else
        {
            await StartAsync();
        }
    }

    private async Task StartAsync()
    {
        SetBusy(true);

        string? exePath = _resolvedExePath;
        bool downloadedNow = false;

        if (exePath is null)
        {
            _theme.SetState(ThemeState.Downloading);

            try
            {
                IToastHandle loading = _toasts.ShowLoading("GoodbyeDPI", "Preparing package…");
                try
                {
                    DownloadResult result = await _packages.DownloadLatestAsync();
                    exePath = result.ExePath;
                    downloadedNow = result.DownloadedNow;
                }
                finally
                {
                    loading.Dismiss();
                }

                _resolvedExePath = exePath;
            }
            catch
            {
                _toasts.Show("Couldn't download GoodbyeDPI", "Checking for a local copy…", NotificationKind.Warning);

                exePath = TryGetLocal();
                if (exePath is null)
                {
                    _theme.SetState(ThemeState.Stopped);
                    const string title = "Can't start GoodbyeDPI";
                    const string description = "The package couldn't be downloaded and no local copy was found. Check your connection and try again.";
                    if (_window.IsVisible)
                    {
                        _dialogs.Show(title, description, kind: NotificationKind.Error);
                    }
                    else
                    {
                        _trayNotify.Show(title, description, NotificationKind.Error);
                    }
                    SetBusy(false);
                    return;
                }
            }
        }

        try
        {
            await _process.Start(exePath);
            _theme.SetState(ThemeState.Running);

            if (downloadedNow)
            {
                _toasts.Show("GoodbyeDPI ready", $"{PackageTag(exePath)} downloaded and started.", NotificationKind.Success);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to start GoodbyeDPI");
            _theme.SetState(ThemeState.Stopped);
            const string title = "GoodbyeDPI";
            const string description = "An error occurred while starting GoodbyeDPI";
            if (_window.IsVisible)
            {
                _dialogs.Show(title, description, kind: NotificationKind.Error);
            }
            else
            {
                _trayNotify.Show(title, description, NotificationKind.Error);
            }
        }
        finally
        {
            SetBusy(false);
        }
    }

    private async Task StopAsync()
    {
        SetBusy(true);
        try
        {
            await _process.Stop();
            _theme.SetState(ThemeState.Stopped);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to stop GoodbyeDPI");
            const string title = "GoodbyeDPI";
            const string description = "An error occurred while stopping GoodbyeDPI";
            if (_window.IsVisible)
            {
                _dialogs.Show(title, description, kind: NotificationKind.Error);
            }
            else
            {
                _trayNotify.Show(title, description, NotificationKind.Error);
            }
        }
        finally
        {
            SetBusy(false);
        }
    }

    private void OnProcessStateChanged(object? sender, bool isRunning)
    {
        if (!isRunning)
        {
            _theme.SetState(ThemeState.Stopped);
        }

        StateChanged?.Invoke(this, new GoodByeDpiStateChangedEventArgs(isRunning, IsBusy));
    }

    private void SetBusy(bool isBusy)
    {
        IsBusy = isBusy;
        StateChanged?.Invoke(this, new GoodByeDpiStateChangedEventArgs(IsRunning, isBusy));
    }

    private string? TryGetLocal()
    {
        try
        {
            return _packages.GetLatestLocalVersion();
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Could not enumerate local packages");
            return null;
        }
    }

    private static string? PackageTag(string exePath) => Path.GetFileName(Path.GetDirectoryName(exePath));
}
