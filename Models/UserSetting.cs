using SQLite;

namespace HereForYou.Models;

/// <summary>
/// Represents a user setting stored as key-value pair.
/// </summary>
[Table("user_settings")]
public class UserSetting
{
    /// <summary>
    /// Gets or sets the setting key (primary key).
    /// </summary>
    [PrimaryKey]
    [Column("key")]
    public string Key { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the setting value as string.
    /// </summary>
    [Column("value")]
    public string Value { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the value type (string, int, bool, json, float).
    /// </summary>
    [Column("value_type")]
    public string ValueType { get; set; } = "string";

    /// <summary>
    /// Gets or sets when this setting was last updated.
    /// </summary>
    [Column("updated_at")]
    public DateTime UpdatedAt { get; set; }
}

/// <summary>
/// Known setting keys.
/// </summary>
public static class SettingKeys
{
    /// <summary>JSON array of keywords to detect.</summary>
    public const string Keywords = "keywords";

    /// <summary>Number of detections before triggering alert.</summary>
    public const string DetectionThreshold = "detection_threshold";

    /// <summary>Time window in seconds for counting detections.</summary>
    public const string ThresholdWindowSeconds = "threshold_window_seconds";

    /// <summary>Whether monitoring is enabled.</summary>
    public const string IsMonitoringEnabled = "is_monitoring_enabled";

    /// <summary>Default alert level.</summary>
    public const string AlertLevelDefault = "alert_level_default";

    /// <summary>Whether to show overlay alerts.</summary>
    public const string EnableOverlayAlerts = "enable_overlay_alerts";

    /// <summary>Whether to play sound alerts.</summary>
    public const string EnableSoundAlerts = "enable_sound_alerts";

    /// <summary>Whether to use vibration.</summary>
    public const string EnableVibration = "enable_vibration";

    /// <summary>Minimum confidence threshold for detection.</summary>
    public const string ConfidenceThreshold = "confidence_threshold";

    /// <summary>User's display name.</summary>
    public const string UserName = "user_name";

    /// <summary>JSON array of family member names.</summary>
    public const string FamilyMemberNames = "family_member_names";
}
