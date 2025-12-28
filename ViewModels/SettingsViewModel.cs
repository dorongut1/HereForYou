using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HereForYou.Services.Interfaces;

namespace HereForYou.ViewModels;

public partial class SettingsViewModel : ObservableObject
{
    private readonly ISettingsService _settings;
    private readonly IDatabaseService _database;

    [ObservableProperty]
    private int _detectionThreshold;

    [ObservableProperty]
    private int _thresholdWindowSeconds;

    [ObservableProperty]
    private float _confidenceThreshold;

    [ObservableProperty]
    private bool _enableOverlayAlerts;

    [ObservableProperty]
    private bool _enableSoundAlerts;

    [ObservableProperty]
    private bool _enableVibration;

    [ObservableProperty]
    private string _userName = string.Empty;

    [ObservableProperty]
    private string _keywords = string.Empty;

    public SettingsViewModel(ISettingsService settings, IDatabaseService database)
    {
        _settings = settings;
        _database = database;
    }

    public async Task InitializeAsync()
    {
        await _settings.InitializeAsync();
        LoadSettings();
    }

    private void LoadSettings()
    {
        DetectionThreshold = _settings.DetectionThreshold;
        ThresholdWindowSeconds = _settings.ThresholdWindowSeconds;
        ConfidenceThreshold = _settings.ConfidenceThreshold;
        EnableOverlayAlerts = _settings.EnableOverlayAlerts;
        EnableSoundAlerts = _settings.EnableSoundAlerts;
        EnableVibration = _settings.EnableVibration;
        UserName = _settings.UserName;
        Keywords = string.Join(", ", _settings.Keywords);
    }

    [RelayCommand]
    private async Task SaveSettingsAsync()
    {
        await _settings.SetDetectionThresholdAsync(DetectionThreshold);
        await _settings.SetThresholdWindowSecondsAsync(ThresholdWindowSeconds);
        await _settings.SetConfidenceThresholdAsync(ConfidenceThreshold);
        await _settings.SetOverlayAlertsEnabledAsync(EnableOverlayAlerts);
        await _settings.SetSoundAlertsEnabledAsync(EnableSoundAlerts);
        await _settings.SetVibrationEnabledAsync(EnableVibration);
        await _settings.SetUserNameAsync(UserName);

        // Parse keywords
        var keywordList = Keywords.Split(',')
            .Select(k => k.Trim())
            .Where(k => !string.IsNullOrEmpty(k))
            .ToList();
        await _settings.SetKeywordsAsync(keywordList);
    }

    [RelayCommand]
    private async Task ResetToDefaultsAsync()
    {
        await _settings.ResetToDefaultsAsync();
        LoadSettings();
    }

    [RelayCommand]
    private async Task ClearAllDataAsync()
    {
        // Clear all data - implement later if needed
        await Task.CompletedTask;
    }
}
