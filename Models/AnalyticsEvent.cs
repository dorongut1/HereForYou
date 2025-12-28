using SQLite;

namespace HereForYou.Models;

/// <summary>
/// Represents an analytics event for tracking app usage.
/// </summary>
[Table("analytics_events")]
public class AnalyticsEvent
{
    /// <summary>
    /// Gets or sets the unique identifier.
    /// </summary>
    [PrimaryKey]
    [AutoIncrement]
    [Column("id")]
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the event type.
    /// </summary>
    [Column("event_type")]
    [Indexed]
    public string EventType { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets additional event data as JSON.
    /// </summary>
    [Column("event_data")]
    public string? EventData { get; set; }

    /// <summary>
    /// Gets or sets when the event occurred.
    /// </summary>
    [Column("timestamp")]
    [Indexed]
    public DateTime Timestamp { get; set; }
}

/// <summary>
/// Analytics event types.
/// </summary>
public static class EventTypes
{
    /// <summary>App was opened.</summary>
    public const string AppOpened = "app_opened";

    /// <summary>App was closed.</summary>
    public const string AppClosed = "app_closed";

    /// <summary>Monitoring started.</summary>
    public const string MonitoringStarted = "monitoring_started";

    /// <summary>Monitoring stopped.</summary>
    public const string MonitoringStopped = "monitoring_stopped";

    /// <summary>Settings were changed.</summary>
    public const string SettingsChanged = "settings_changed";

    /// <summary>Alert was shown.</summary>
    public const string AlertShown = "alert_shown";

    /// <summary>Alert was acknowledged.</summary>
    public const string AlertAcknowledged = "alert_acknowledged";

    /// <summary>Keyword was detected.</summary>
    public const string KeywordDetected = "keyword_detected";

    /// <summary>Screen session started.</summary>
    public const string ScreenSessionStarted = "screen_session_started";

    /// <summary>Screen session ended.</summary>
    public const string ScreenSessionEnded = "screen_session_ended";
}
