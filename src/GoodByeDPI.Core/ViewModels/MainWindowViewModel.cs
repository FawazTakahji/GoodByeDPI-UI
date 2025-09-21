using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GoodByeDPI.Core.Services;

namespace GoodByeDPI.Core.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    private readonly SettingsProvider _settingsProvider;
    private readonly IMessageBoxService _messageBoxService;
    public ProcessManager ProcessManager { get; }

    [ObservableProperty]
    private string _executablePath = string.Empty;
    [ObservableProperty]
    private string _commandLineArguments = string.Empty;

    [ObservableProperty]
    private bool _settingsChanged;

    public MainWindowViewModel(SettingsProvider settingsProvider,
        IMessageBoxService messageBoxService,
        ProcessManager processManager)
    {
        _settingsProvider = settingsProvider;
        _messageBoxService = messageBoxService;
        ProcessManager = processManager;

        ExecutablePath = settingsProvider.Settings.ExecutablePath;
        CommandLineArguments = settingsProvider.Settings.CommandLineArguments;
    }

    [RelayCommand]
    private void Save()
    {
        if (ProcessManager.IsRunning)
        {
            Stop();
        }

        string oldExecutablePath = _settingsProvider.Settings.ExecutablePath;
        string oldCommandLineArguments = _settingsProvider.Settings.CommandLineArguments;
        _settingsProvider.Settings.ExecutablePath = ExecutablePath;
        _settingsProvider.Settings.CommandLineArguments = CommandLineArguments;

        try
        {
            _settingsProvider.Save();
            SettingsChanged = false;
        }
        catch (Exception ex)
        {
            _messageBoxService.Show(
                Constants.Title,
                $"An error occurred while saving settings:{Environment.NewLine}{ex}",
                Button.Ok,
                Icon.Error);
            _settingsProvider.Settings.ExecutablePath = oldExecutablePath;
            _settingsProvider.Settings.CommandLineArguments = oldCommandLineArguments;
        }
    }

    [RelayCommand]
    private void Reset()
    {
        ExecutablePath = _settingsProvider.Settings.ExecutablePath;
        CommandLineArguments = _settingsProvider.Settings.CommandLineArguments;
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
                "Please enter an executable path",
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

    partial void OnExecutablePathChanged(string value)
    {
        if (!_settingsProvider.Settings.ExecutablePath.Equals(value, StringComparison.OrdinalIgnoreCase))
        {
            SettingsChanged = true;
        }
        else if (_settingsProvider.Settings.CommandLineArguments.Equals(CommandLineArguments, StringComparison.OrdinalIgnoreCase))
        {
            SettingsChanged = false;
        }
    }

    partial void OnCommandLineArgumentsChanged(string value)
    {
        if (!_settingsProvider.Settings.CommandLineArguments.Equals(value, StringComparison.OrdinalIgnoreCase))
        {
            SettingsChanged = true;
        }
        else if(_settingsProvider.Settings.ExecutablePath.Equals(ExecutablePath, StringComparison.OrdinalIgnoreCase))
        {
            SettingsChanged = false;
        }
    }

#if DEBUG
    public MainWindowViewModel()
    {
        _settingsProvider = new SettingsProvider();
        ProcessManager = new ProcessManager();
        ExecutablePath = @"C:\Test\Test.exe";
        CommandLineArguments = "--chicken --noodles";
    }
#endif
}