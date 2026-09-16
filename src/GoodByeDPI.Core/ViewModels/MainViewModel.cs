using CommunityToolkit.Mvvm.Input;
using GoodByeDPI.Core.Navigation;

namespace GoodByeDPI.Core.ViewModels;

public partial class MainViewModel : ViewModelBase, INavigable
{
    private NavigationService _navigation;

    public MainViewModel(NavigationService navigationService)
    {
        _navigation = navigationService;
    }

    [RelayCommand]
    private void GoToSettings()
    {
        _navigation.NavigateTo<SettingsViewModel>();
    }
}