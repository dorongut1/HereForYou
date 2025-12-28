using HereForYou.Models;

namespace HereForYou.Services.Interfaces;

/// <summary>
/// Interface for application settings management.
/// </summary>
public interface ISettingsService
{
    /// <summary>
    /// Event raised when a setting changes.
    /// </summary>
    event EventHandler<SettingChangedEventArgs>? SettingChanged;

    /// <summary>
    /// Gets the detection threshold (number of detections before alert).
    /// </summary>
    int DetectionThreshold { get; }

    /// <summary>
    /// Gets the threshold window in seconds.
    /// </summary>
    int ThresholdWindowSeconds { get; }

    /// <summary>
    /// Gets a value indicating whether monitoring is enabled.
    /// </summary>
    bool IsMonitoringEnabled { get; }

    /// <summary>
    /// Gets a value indicating whether overlay alerts are enabled.
    /// </summary>
    bool EnableOverlayAlerts { get; }

    /// <summary>
    /// Gets a value indicating whether sound alerts are enabled.
    /// </summary>
    bool EnableSoundAlerts { get; }

    /// <summary>
    /// Gets a value indicating whether vibration is enabled.
    /// </summary>
    bool EnableVibration { get; }

    /// <summary>
    /// Gets the confidence threshold for detection.
    /// </summary>
    float ConfidenceThreshold { get; }

    /// <summary>
    /// Gets the list of keywords to detect.
    /// </summary>
    List<string> Keywords { get; }

    /// <summary>
    /// Gets the user's display name.
    /// </summary>
    string UserName { get; }

    /// <summary>
    /// Initializes settings from database.
    /// </summary>
    Task InitializeAsync();

    /// <summary>
    /// Sets the detection threshold.
    /// </summary>
    Task SetDetectionThresholdAsync(int value);

    /// <summary>
    /// Sets the threshold window.
    /// </summary>
    Task SetThresholdWindowSecondsAsync(int value);

    /// <summary>
    /// Sets whether monitoring is enabled.
    /// </summary>
    Task SetMonitoringEnabledAsync(bool value);

    /// <summary>
    /// Sets whether overlay alerts are enabled.
    /// </summary>
    Task SetOverlayAlertsEnabledAsync(bool value);

    /// <summary>
    /// Sets whether sound alerts are enabled.
    /// </summary>
    Task SetSoundAlertsEnabledAsync(bool value);

    /// <summary>
    /// Sets whether vibration is enabled.
    /// </summary>
    Task SetVibrationEnabledAsync(bool value);

    /// <summary>
    /// Sets the confidence threshold.
    /// </summary>
    Task SetConfidenceThresholdAsync(float value);

    /// <summary>
    /// Sets the keywords to detect.
    /// </summary>
    Task SetKeywordsAsync(List<string> keywords);

    /// <summary>
    /// Sets the user's display name.
    /// </summary>
    Task SetUserNameAsync(string name);

    /// <summary>
    /// Resets all settings to defaults.
    /// </summary>
    Task ResetToDefaultsAsync();
}

/// <summary>
/// Event arguments for setting changes.
/// </summary>
public class SettingChangedEventArgs : EventArgs
{
    /// <summary>
    /// Gets the setting key.
    /// </summary>
    public string Key { get; }

    /// <summary>
    /// Gets the old value.
    /// </summary>
    public object? OldValue { get; }

    /// <summary>
    /// Gets the new value.
    /// </summary>
    public object? NewValue { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="SettingChangedEventArgs"/> class.
    /// </summary>
    public SettingChangedEventArgs(string key, object? oldValue, object? newValue)
    {
        Key = key;
        OldValue = oldValue;
        NewValue = newValue;
    }
}
