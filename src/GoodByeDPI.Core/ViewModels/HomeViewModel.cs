using CommunityToolkit.Mvvm.Input;
using GoodByeDPI.Core.Navigation;

namespace GoodByeDPI.Core.ViewModels;

public partial class HomeViewModel : ViewModelBase, INavigable
{
    private readonly NavigationService _navigation;

    public HomeViewModel(NavigationService navigationService)
    {
        _navigation = navigationService;
    }

    [RelayCommand]
    private void GoToSettings()
    {
        _navigation.NavigateTo<SettingsViewModel>();
    }
}