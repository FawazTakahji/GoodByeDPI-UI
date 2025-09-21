using System.Text.Json;
using CommunityToolkit.Mvvm.ComponentModel;
using GoodByeDPI.Core.Models;

namespace GoodByeDPI.Core.Services;

public partial class SettingsProvider : ObservableObject
{
    private readonly string _settingsPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "settings.json");
    [ObservableProperty]
    private Settings _settings = new();

    public void Load()
    {
        if (File.Exists(_settingsPath))
        {
            string json = File.ReadAllText(_settingsPath);
            Settings = JsonSerializer.Deserialize<Settings>(json) ?? new();
        }
    }

    public void Save()
    {
        string json = JsonSerializer.Serialize(Settings);
        File.WriteAllText(_settingsPath, json);
    }
}