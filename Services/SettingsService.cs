using HereForYou.Models;
using HereForYou.Services.Interfaces;

namespace HereForYou.Services;

/// <summary>
/// Service for managing application settings.
/// </summary>
public class SettingsService : ISettingsService
{
    private readonly IDatabaseService _database;
    private bool _isInitialized;

    // Cached values
    private int _detectionThreshold = 3;
    private int _thresholdWindowSeconds = 30;
    private bool _isMonitoringEnabled;
    private bool _enableOverlayAlerts = true;
    private bool _enableSoundAlerts = true;
    private bool _enableVibration = true;
    private float _confidenceThreshold = 0.7f;
    private List<string> _keywords = new() { "אמא", "אבא" };
    private string _userName = string.Empty;

    /// <inheritdoc/>
    public event EventHandler<SettingChangedEventArgs>? SettingChanged;

    /// <inheritdoc/>
    public int DetectionThreshold => _detectionThreshold;

    /// <inheritdoc/>
    public int ThresholdWindowSeconds => _thresholdWindowSeconds;

    /// <inheritdoc/>
    public bool IsMonitoringEnabled => _isMonitoringEnabled;

    /// <inheritdoc/>
    public bool EnableOverlayAlerts => _enableOverlayAlerts;

    /// <inheritdoc/>
    public bool EnableSoundAlerts => _enableSoundAlerts;

    /// <inheritdoc/>
    public bool EnableVibration => _enableVibration;

    /// <inheritdoc/>
    public float ConfidenceThreshold => _confidenceThreshold;

    /// <inheritdoc/>
    public List<string> Keywords => _keywords;

    /// <inheritdoc/>
    public string UserName => _userName;

    /// <summary>
    /// Initializes a new instance of the <see cref="SettingsService"/> class.
    /// </summary>
    public SettingsService(IDatabaseService database)
    {
        _database = database;
    }

    /// <inheritdoc/>
    public async Task InitializeAsync()
    {
        if (_isInitialized)
        {
            return;
        }

        await LoadSettingsAsync().ConfigureAwait(false);
        _isInitialized = true;
    }

    private async Task LoadSettingsAsync()
    {
        _detectionThreshold = await _database.GetSettingAsync<int>(SettingKeys.DetectionThreshold).ConfigureAwait(false);
        if (_detectionThreshold == 0)
        {
            _detectionThreshold = 3;
        }

        _thresholdWindowSeconds = await _database.GetSettingAsync<int>(SettingKeys.ThresholdWindowSeconds).ConfigureAwait(false);
        if (_thresholdWindowSeconds == 0)
        {
            _thresholdWindowSeconds = 30;
        }

        _isMonitoringEnabled = await _database.GetSettingAsync<bool>(SettingKeys.IsMonitoringEnabled).ConfigureAwait(false);
        _enableOverlayAlerts = await _database.GetSettingAsync<bool>(SettingKeys.EnableOverlayAlerts).ConfigureAwait(false);
        _enableSoundAlerts = await _database.GetSettingAsync<bool>(SettingKeys.EnableSoundAlerts).ConfigureAwait(false);
        _enableVibration = await _database.GetSettingAsync<bool>(SettingKeys.EnableVibration).ConfigureAwait(false);

        _confidenceThreshold = await _database.GetSettingAsync<float>(SettingKeys.ConfidenceThreshold).ConfigureAwait(false);
        if (_confidenceThreshold == 0)
        {
            _confidenceThreshold = 0.7f;
        }

        var keywords = await _database.GetSettingAsync<List<string>>(SettingKeys.Keywords).ConfigureAwait(false);
        if (keywords != null && keywords.Count > 0)
        {
            _keywords = keywords;
        }

        _userName = await _database.GetSettingAsync<string>(SettingKeys.UserName).ConfigureAwait(false) ?? string.Empty;
    }

    /// <inheritdoc/>
    public async Task SetDetectionThresholdAsync(int value)
    {
        var oldValue = _detectionThreshold;
        _detectionThreshold = value;
        await _database.SetSettingAsync(SettingKeys.DetectionThreshold, value).ConfigureAwait(false);
        OnSettingChanged(SettingKeys.DetectionThreshold, oldValue, value);
    }

    /// <inheritdoc/>
    public async Task SetThresholdWindowSecondsAsync(int value)
    {
        var oldValue = _thresholdWindowSeconds;
        _thresholdWindowSeconds = value;
        await _database.SetSettingAsync(SettingKeys.ThresholdWindowSeconds, value).ConfigureAwait(false);
        OnSettingChanged(SettingKeys.ThresholdWindowSeconds, oldValue, value);
    }

    /// <inheritdoc/>
    public async Task SetMonitoringEnabledAsync(bool value)
    {
        var oldValue = _isMonitoringEnabled;
        _isMonitoringEnabled = value;
        await _database.SetSettingAsync(SettingKeys.IsMonitoringEnabled, value).ConfigureAwait(false);
        OnSettingChanged(SettingKeys.IsMonitoringEnabled, oldValue, value);
    }

    /// <inheritdoc/>
    public async Task SetOverlayAlertsEnabledAsync(bool value)
    {
        var oldValue = _enableOverlayAlerts;
        _enableOverlayAlerts = value;
        await _database.SetSettingAsync(SettingKeys.EnableOverlayAlerts, value).ConfigureAwait(false);
        OnSettingChanged(SettingKeys.EnableOverlayAlerts, oldValue, value);
    }

    /// <inheritdoc/>
    public async Task SetSoundAlertsEnabledAsync(bool value)
    {
        var oldValue = _enableSoundAlerts;
        _enableSoundAlerts = value;
        await _database.SetSettingAsync(SettingKeys.EnableSoundAlerts, value).ConfigureAwait(false);
        OnSettingChanged(SettingKeys.EnableSoundAlerts, oldValue, value);
    }

    /// <inheritdoc/>
    public async Task SetVibrationEnabledAsync(bool value)
    {
        var oldValue = _enableVibration;
        _enableVibration = value;
        await _database.SetSettingAsync(SettingKeys.EnableVibration, value).ConfigureAwait(false);
        OnSettingChanged(SettingKeys.EnableVibration, oldValue, value);
    }

    /// <inheritdoc/>
    public async Task SetConfidenceThresholdAsync(float value)
    {
        var oldValue = _confidenceThreshold;
        _confidenceThreshold = value;
        await _database.SetSettingAsync(SettingKeys.ConfidenceThreshold, value).ConfigureAwait(false);
        OnSettingChanged(SettingKeys.ConfidenceThreshold, oldValue, value);
    }

    /// <inheritdoc/>
    public async Task SetKeywordsAsync(List<string> keywords)
    {
        var oldValue = _keywords;
        _keywords = keywords;
        await _database.SetSettingAsync(SettingKeys.Keywords, keywords).ConfigureAwait(false);
        OnSettingChanged(SettingKeys.Keywords, oldValue, keywords);
    }

    /// <inheritdoc/>
    public async Task SetUserNameAsync(string name)
    {
        var oldValue = _userName;
        _userName = name;
        await _database.SetSettingAsync(SettingKeys.UserName, name).ConfigureAwait(false);
        OnSettingChanged(SettingKeys.UserName, oldValue, name);
    }

    /// <inheritdoc/>
    public async Task ResetToDefaultsAsync()
    {
        await SetDetectionThresholdAsync(3).ConfigureAwait(false);
        await SetThresholdWindowSecondsAsync(30).ConfigureAwait(false);
        await SetMonitoringEnabledAsync(false).ConfigureAwait(false);
        await SetOverlayAlertsEnabledAsync(true).ConfigureAwait(false);
        await SetSoundAlertsEnabledAsync(true).ConfigureAwait(false);
        await SetVibrationEnabledAsync(true).ConfigureAwait(false);
        await SetConfidenceThresholdAsync(0.7f).ConfigureAwait(false);
        await SetKeywordsAsync(new List<string> { "אמא", "אבא" }).ConfigureAwait(false);
        await SetUserNameAsync(string.Empty).ConfigureAwait(false);
    }

    private void OnSettingChanged(string key, object? oldValue, object? newValue)
    {
        SettingChanged?.Invoke(this, new SettingChangedEventArgs(key, oldValue, newValue));
    }
}
