using CommunityToolkit.Mvvm.Input;
using GoodByeDPI.Core.Navigation;

namespace GoodByeDPI.Core.ViewModels;

public partial class SettingsViewModel : ViewModelBase, INavigable
{
    private readonly NavigationService _navigation;

    public SettingsViewModel(NavigationService navigation)
    {
        _navigation = navigation;
    }

    [RelayCommand]
    private void GoMain()
    {
        _navigation.NavigateTo<HomeViewModel>();
    }
}