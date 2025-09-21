using CommunityToolkit.Mvvm.ComponentModel;

namespace GoodByeDPI.Core.Models;

public partial class Settings : ObservableObject
{
    [ObservableProperty]
    private string _executablePath = string.Empty;
    [ObservableProperty]
    private string _commandLineArguments = string.Empty;
}