using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GoodByeDPI.Core.Navigation;
using GoodByeDPI.Core.Processes;

namespace GoodByeDPI.Core.ViewModels;

public partial class HomeViewModel : ViewModelBase, INavigable
{
    private readonly GoodByeDpiService _service;
    private readonly NavigationService _navigation;

    [ObservableProperty, NotifyCanExecuteChangedFor(nameof(ToggleCommand))]
    public partial bool IsRunning { get; set; }

    [ObservableProperty, NotifyCanExecuteChangedFor(nameof(ToggleCommand))]
    public partial bool IsBusy { get; set; }

    public HomeViewModel(GoodByeDpiService service, NavigationService navigation)
    {
        _service = service;
        _navigation = navigation;

        IsRunning = service.IsRunning;
        IsBusy = service.IsBusy;

        service.StateChanged += OnStateChanged;
    }

    private void OnStateChanged(object? sender, GoodByeDpiStateChangedEventArgs e)
    {
        IsRunning = e.IsRunning;
        IsBusy = e.IsBusy;
    }

    [RelayCommand(CanExecute = nameof(CanToggle))]
    private Task Toggle() => _service.ToggleAsync();

    private bool CanToggle() => !IsBusy;

    [RelayCommand]
    private void GoToSettings() => _navigation.NavigateTo<SettingsViewModel>();
}