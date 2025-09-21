using CommunityToolkit.Mvvm.Input;
using GoodByeDPI.Core.Services;

namespace GoodByeDPI.Core.ViewModels;

public partial class AppViewModel : ViewModelBase
{
    private readonly SettingsProvider _settingsProvider;
    private readonly IMessageBoxService _messageBoxService;
    public ProcessManager ProcessManager { get; }

    public AppViewModel(SettingsProvider settingsProvider,
        IMessageBoxService messageBoxService,
        ProcessManager processManager,
        IArgsProvider argsProvider)
    {
        _settingsProvider = settingsProvider;
        _messageBoxService = messageBoxService;
        ProcessManager = processManager;

        TryLoadSettings();

        if (argsProvider.Args.Any(s => s.Equals("--auto-start", StringComparison.OrdinalIgnoreCase)))
        {
            Start();
        }
    }

    private void TryLoadSettings()
    {
        try
        {
            _settingsProvider.Load();
        }
        catch (Exception ex)
        {
            _messageBoxService.Show(
                Constants.Title,
                $"An error occurred while loading settings:{Environment.NewLine}{ex}",
                Button.Ok,
                Icon.Error);
        }
    }

    [RelayCommand]
    private void Toggle()
    {
        if (ProcessManager.IsRunning)
        {
            Stop();
        }
        else
        {
            Start();
        }
    }

    private void Start()
    {
        if (_settingsProvider.Settings.ExecutablePath.Length < 1)
        {
            _messageBoxService.Show(
                Constants.Title,
                "Please set the executable path in the settings.",
                Button.Ok,
                Icon.Warning);
            return;
        }

        try
        {
            ProcessManager.Start(_settingsProvider.Settings.ExecutablePath, _settingsProvider.Settings.CommandLineArguments);
        }
        catch (FileNotFoundException)
        {
            _messageBoxService.Show(
                Constants.Title,
                $"The executable at the path '{_settingsProvider.Settings.ExecutablePath}' could not be found.",
                Button.Ok,
                Icon.Error);
        }
        catch (Exception ex)
        {
            _messageBoxService.Show(
                Constants.Title,
                $"An error occurred while starting the process:{Environment.NewLine}{ex}",
                Button.Ok,
                Icon.Error);
        }
    }

    private void Stop()
    {
        try
        {
            ProcessManager.Stop();
        }
        catch (Exception ex)
        {
            _messageBoxService.Show(
                Constants.Title,
                $"An error occurred while stopping the process:{Environment.NewLine}{ex}",
                Button.Ok,
                Icon.Error);
        }
    }

#if DEBUG
    public AppViewModel()
    {

    }
#endif
}