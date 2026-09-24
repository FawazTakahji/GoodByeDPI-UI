using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GoodByeDPI.Core.Dialogs;
using GoodByeDPI.Core.Navigation;
using GoodByeDPI.Core.Packages;
using GoodByeDPI.Core.Processes;
using GoodByeDPI.Core.Theming;

namespace GoodByeDPI.Core.ViewModels;

public partial class HomeViewModel : ViewModelBase, INavigable
{
    private readonly NavigationService _navigation;
    private readonly IDialogService _dialogs;
    private readonly IToastService _toasts;
    private readonly PackageManager _packages;
    private readonly IThemeService _theme;
    private readonly ProcessManager _process;

    [ObservableProperty, NotifyCanExecuteChangedFor(nameof(ToggleCommand))]
    public partial bool IsRunning { get; set; }

    [ObservableProperty, NotifyCanExecuteChangedFor(nameof(ToggleCommand))]
    public partial bool IsBusy { get; set; }

    public HomeViewModel(
        NavigationService navigationService,
        IDialogService dialogs,
        IToastService toasts,
        PackageManager packages,
        IThemeService theme,
        ProcessManager process)
    {
        _navigation = navigationService;
        _dialogs = dialogs;
        _toasts = toasts;
        _packages = packages;
        _theme = theme;
        _process = process;

        IsRunning = _process.IsRunning;
        _process.StateChanged += OnProcessStateChanged;
    }

    private void OnProcessStateChanged(object? sender, bool isRunning)
    {
        IsRunning = isRunning;
        if (!isRunning)
        {
            _theme.SetState(ThemeState.Stopped);
        }
    }

    [RelayCommand(CanExecute = nameof(CanToggle))]
    private async Task Toggle()
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
        IsBusy = true;
        _theme.SetState(ThemeState.Downloading);

        string? exePath;
        bool downloadedNow = false;

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
        }
        catch
        {
            _toasts.Show("Couldn't download GoodbyeDPI", "Checking for a local copy…", NotificationKind.Warning);

            exePath = _packages.GetLatestLocalVersion();
            if (exePath is null)
            {
                _theme.SetState(ThemeState.Stopped);
                await _dialogs.ShowModal(
                    "Can't start GoodbyeDPI",
                    "The package couldn't be downloaded and no local copy was found. Check your connection and try again.",
                    kind: NotificationKind.Error);
                IsBusy = false;
                return;
            }
        }

        try
        {
            await _process.Start(exePath);
            IsRunning = true;
            _theme.SetState(ThemeState.Running);

            if (downloadedNow)
            {
                _toasts.Show("GoodbyeDPI ready", $"{PackageTag(exePath)} downloaded and started.", NotificationKind.Success);
            }
        }
        catch (Exception ex)
        {
            _theme.SetState(ThemeState.Stopped);
            await _dialogs.ShowModal("Failed to start GoodbyeDPI", ex.Message, kind: NotificationKind.Error);
        }
        finally
        {
            IsBusy = false;
        }
    }

    private async Task StopAsync()
    {
        IsBusy = true;
        try
        {
            await _process.Stop();
            IsRunning = false;
            _theme.SetState(ThemeState.Stopped);
        }
        catch (Exception ex)
        {
            await _dialogs.ShowModal("Failed to stop GoodbyeDPI", ex.Message, kind: NotificationKind.Error);
        }
        finally
        {
            IsBusy = false;
        }
    }

    private bool CanToggle() => !IsBusy;

    [RelayCommand]
    private void GoToSettings()
    {
        _navigation.NavigateTo<SettingsViewModel>();
    }

    private static string? PackageTag(string exePath) => Path.GetFileName(Path.GetDirectoryName(exePath));
}