using CommunityToolkit.Mvvm.Input;
using GoodByeDPI.Core.Dialogs;
using GoodByeDPI.Core.Navigation;
using GoodByeDPI.Core.Packages;
using GoodByeDPI.Core.Theming;

namespace GoodByeDPI.Core.ViewModels;

public partial class HomeViewModel : ViewModelBase, INavigable
{
    private readonly NavigationService _navigation;
    private readonly IDialogService _dialogs;
    private readonly IToastService _toasts;
    private readonly PackageManager _packages;
    private readonly IThemeService _theme;

    public HomeViewModel(NavigationService navigationService, IDialogService dialogs, IToastService toasts, PackageManager packages, IThemeService theme)
    {
        _navigation = navigationService;
        _dialogs = dialogs;
        _toasts = toasts;
        _packages = packages;
        _theme = theme;
    }

    [RelayCommand]
    private async Task Start()
    {
        _theme.SetState(ThemeState.Downloading);

        DownloadResult result;
        try
        {
            IToastHandle loading = _toasts.ShowLoading("GoodbyeDPI", "Preparing package…");
            try
            {
                result = await _packages.DownloadLatestAsync();
            }
            finally
            {
                loading.Dismiss();
            }
        }
        catch
        {
            _toasts.Show("Couldn't download GoodbyeDPI", "Checking for a local copy…", NotificationKind.Warning);

            string? localExe = _packages.GetLatestLocalVersion();
            if (localExe is null)
            {
                _theme.SetState(ThemeState.Stopped);
                await _dialogs.ShowModal(
                    "Can't start GoodbyeDPI",
                    "The package couldn't be downloaded and no local copy was found. Check your connection and try again.",
                    kind: NotificationKind.Error);
                return;
            }

            _theme.SetState(ThemeState.Running);
            _toasts.Show("GoodbyeDPI ready", $"Using local package {PackageTag(localExe)}", NotificationKind.Success);
            return;
        }

        _theme.SetState(ThemeState.Running);
        if (result.DownloadedNow)
        {
            _toasts.Show("GoodbyeDPI ready", $"{PackageTag(result.ExePath)} downloaded and installed.", NotificationKind.Success);
        }
    }

    [RelayCommand]
    private void GoToSettings()
    {
        _navigation.NavigateTo<SettingsViewModel>();
    }

    private static string? PackageTag(string exePath) => Path.GetFileName(Path.GetDirectoryName(exePath));
}