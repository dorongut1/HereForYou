using SQLite;

namespace HereForYou.Models;

/// <summary>
/// Represents an alert sent to the parent after multiple detections.
/// </summary>
[Table("alerts")]
public class Alert
{
    /// <summary>
    /// Gets or sets the unique identifier.
    /// </summary>
    [PrimaryKey]
    [AutoIncrement]
    [Column("id")]
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets when the alert was created.
    /// </summary>
    [Column("created_at")]
    [Indexed]
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Gets or sets the keyword that triggered the alert.
    /// </summary>
    [Column("keyword")]
    public string Keyword { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the number of detections that triggered this alert.
    /// </summary>
    [Column("detection_count")]
    public int DetectionCount { get; set; }

    /// <summary>
    /// Gets or sets the alert level (Info, Warning, Critical).
    /// </summary>
    [Column("alert_level")]
    public string AlertLevel { get; set; } = "Warning";

    /// <summary>
    /// Gets or sets a value indicating whether the alert was acknowledged.
    /// </summary>
    [Column("was_acknowledged")]
    [Indexed]
    public bool WasAcknowledged { get; set; }

    /// <summary>
    /// Gets or sets when the alert was acknowledged.
    /// </summary>
    [Column("acknowledged_at")]
    public DateTime? AcknowledgedAt { get; set; }

    /// <summary>
    /// Gets or sets how the alert was acknowledged (handled, snoozed, dismissed).
    /// </summary>
    [Column("acknowledgment_type")]
    public string? AcknowledgmentType { get; set; }

    /// <summary>
    /// Gets or sets the time window in seconds during which detections occurred.
    /// </summary>
    [Column("time_window_seconds")]
    public int TimeWindowSeconds { get; set; } = 30;

    /// <summary>
    /// Gets the time it took to acknowledge the alert.
    /// </summary>
    [Ignore]
    public TimeSpan? AcknowledgmentTime => WasAcknowledged && AcknowledgedAt.HasValue
        ? AcknowledgedAt.Value - CreatedAt
        : null;
}

/// <summary>
/// Alert severity levels.
/// </summary>
public enum AlertLevelType
{
    /// <summary>Informational alert - first detection.</summary>
    Info,

    /// <summary>Warning alert - second detection.</summary>
    Warning,

    /// <summary>Critical alert - three or more detections.</summary>
    Critical
}

/// <summary>
/// How the user acknowledged the alert.
/// </summary>
public enum AcknowledgmentType
{
    /// <summary>User handled the situation.</summary>
    Handled,

    /// <summary>User requested more time.</summary>
    Snoozed,

    /// <summary>User dismissed without action.</summary>
    Dismissed
}
