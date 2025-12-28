using SQLite;

namespace HereForYou.Models;

/// <summary>
/// Represents a voice detection event when a keyword is recognized.
/// </summary>
[Table("detection_events")]
public class DetectionEvent
{
    /// <summary>
    /// Gets or sets the unique identifier.
    /// </summary>
    [PrimaryKey]
    [AutoIncrement]
    [Column("id")]
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the timestamp when the keyword was detected.
    /// </summary>
    [Column("timestamp")]
    [Indexed]
    public DateTime Timestamp { get; set; }

    /// <summary>
    /// Gets or sets the detected keyword (e.g., "אמא", "אבא").
    /// </summary>
    [Column("keyword")]
    [Indexed]
    public string Keyword { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the confidence level of the detection (0.0 - 1.0).
    /// </summary>
    [Column("confidence")]
    public float Confidence { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the parent responded to this detection.
    /// </summary>
    [Column("was_responded_to")]
    [Indexed]
    public bool WasRespondedTo { get; set; }

    /// <summary>
    /// Gets or sets the response time in seconds (null if not responded).
    /// </summary>
    [Column("response_time_seconds")]
    public int? ResponseTimeSeconds { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether this detection was part of an alert.
    /// </summary>
    [Column("was_part_of_alert")]
    public bool WasPartOfAlert { get; set; }

    /// <summary>
    /// Gets or sets the associated alert ID (if part of an alert).
    /// </summary>
    [Column("alert_id")]
    public int? AlertId { get; set; }

    /// <summary>
    /// Gets or sets additional context as JSON string.
    /// </summary>
    [Column("context")]
    public string? Context { get; set; }

    /// <summary>
    /// Gets the response time as TimeSpan.
    /// </summary>
    [Ignore]
    public TimeSpan? ResponseTime => ResponseTimeSeconds.HasValue
        ? TimeSpan.FromSeconds(ResponseTimeSeconds.Value)
        : null;
}
