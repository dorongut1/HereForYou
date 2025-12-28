using SQLite;

namespace HereForYou.Models;

/// <summary>
/// Represents a screen usage session.
/// </summary>
[Table("screen_time_sessions")]
public class ScreenTimeSession
{
    /// <summary>
    /// Gets or sets the unique identifier.
    /// </summary>
    [PrimaryKey]
    [AutoIncrement]
    [Column("id")]
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets when the session started.
    /// </summary>
    [Column("start_time")]
    [Indexed]
    public DateTime StartTime { get; set; }

    /// <summary>
    /// Gets or sets when the session ended (null if still active).
    /// </summary>
    [Column("end_time")]
    public DateTime? EndTime { get; set; }

    /// <summary>
    /// Gets or sets the duration in seconds.
    /// </summary>
    [Column("duration_seconds")]
    public int? DurationSeconds { get; set; }

    /// <summary>
    /// Gets or sets the name of the active application.
    /// </summary>
    [Column("app_name")]
    public string AppName { get; set; } = "General";

    /// <summary>
    /// Gets or sets a value indicating whether a detection occurred during this session.
    /// </summary>
    [Column("was_interrupted")]
    public bool WasInterrupted { get; set; }

    /// <summary>
    /// Gets or sets the number of detections during this session.
    /// </summary>
    [Column("interruption_count")]
    public int InterruptionCount { get; set; }

    /// <summary>
    /// Gets or sets the device type (Mobile, Desktop, Tablet).
    /// </summary>
    [Column("device_type")]
    public string DeviceType { get; set; } = "Mobile";

    /// <summary>
    /// Gets the session duration.
    /// </summary>
    [Ignore]
    public TimeSpan Duration => EndTime.HasValue
        ? EndTime.Value - StartTime
        : DateTime.Now - StartTime;

    /// <summary>
    /// Gets a value indicating whether the session is still active.
    /// </summary>
    [Ignore]
    public bool IsActive => !EndTime.HasValue;
}
