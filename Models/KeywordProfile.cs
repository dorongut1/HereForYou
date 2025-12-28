using SQLite;

namespace HereForYou.Models;

/// <summary>
/// Represents a custom keyword profile for voice detection.
/// </summary>
[Table("keyword_profiles")]
public class KeywordProfile
{
    /// <summary>
    /// Gets or sets the unique identifier.
    /// </summary>
    [PrimaryKey]
    [AutoIncrement]
    [Column("id")]
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the keyword identifier (e.g., "ima", "abba").
    /// </summary>
    [Column("keyword")]
    [Unique]
    public string Keyword { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the display name (e.g., "אמא", "אבא").
    /// </summary>
    [Column("display_name")]
    public string DisplayName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the path to the Picovoice model file.
    /// </summary>
    [Column("model_path")]
    public string? ModelPath { get; set; }

    /// <summary>
    /// Gets or sets the detection sensitivity (0.0 - 1.0).
    /// </summary>
    [Column("sensitivity")]
    public float Sensitivity { get; set; } = 0.7f;

    /// <summary>
    /// Gets or sets a value indicating whether this keyword is enabled.
    /// </summary>
    [Column("is_enabled")]
    public bool IsEnabled { get; set; } = true;

    /// <summary>
    /// Gets or sets the path to a voice sample for training.
    /// </summary>
    [Column("voice_sample_path")]
    public string? VoiceSamplePath { get; set; }

    /// <summary>
    /// Gets or sets when this profile was created.
    /// </summary>
    [Column("created_at")]
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Gets or sets when this profile was last updated.
    /// </summary>
    [Column("updated_at")]
    public DateTime UpdatedAt { get; set; }
}
