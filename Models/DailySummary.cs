using SQLite;

namespace HereForYou.Models;

/// <summary>
/// Represents a daily summary of usage statistics.
/// </summary>
[Table("daily_summaries")]
public class DailySummary
{
    /// <summary>
    /// Gets or sets the unique identifier.
    /// </summary>
    [PrimaryKey]
    [AutoIncrement]
    [Column("id")]
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the date for this summary.
    /// </summary>
    [Column("date")]
    [Indexed]
    [Unique]
    public DateTime Date { get; set; }

    /// <summary>
    /// Gets or sets the total screen time in seconds.
    /// </summary>
    [Column("total_screen_time_seconds")]
    public int TotalScreenTimeSeconds { get; set; }

    /// <summary>
    /// Gets or sets the total number of detections.
    /// </summary>
    [Column("total_detections")]
    public int TotalDetections { get; set; }

    /// <summary>
    /// Gets or sets the total number of alerts.
    /// </summary>
    [Column("total_alerts")]
    public int TotalAlerts { get; set; }

    /// <summary>
    /// Gets or sets the number of detections that were responded to.
    /// </summary>
    [Column("responded_detections")]
    public int RespondedDetections { get; set; }

    /// <summary>
    /// Gets or sets the average response time in seconds.
    /// </summary>
    [Column("average_response_time_seconds")]
    public int? AverageResponseTimeSeconds { get; set; }

    /// <summary>
    /// Gets or sets the presence score (0-100).
    /// </summary>
    [Column("presence_score")]
    public float? PresenceScore { get; set; }

    /// <summary>
    /// Gets or sets the longest screen session in seconds.
    /// </summary>
    [Column("longest_screen_session_seconds")]
    public int? LongestScreenSessionSeconds { get; set; }

    /// <summary>
    /// Gets or sets when this summary was created.
    /// </summary>
    [Column("created_at")]
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Gets or sets when this summary was last updated.
    /// </summary>
    [Column("updated_at")]
    public DateTime UpdatedAt { get; set; }

    /// <summary>
    /// Gets the total screen time as TimeSpan.
    /// </summary>
    [Ignore]
    public TimeSpan TotalScreenTime => TimeSpan.FromSeconds(TotalScreenTimeSeconds);

    /// <summary>
    /// Gets the response rate as percentage.
    /// </summary>
    [Ignore]
    public double ResponseRate => TotalDetections > 0
        ? (double)RespondedDetections / TotalDetections * 100
        : 0;
}
