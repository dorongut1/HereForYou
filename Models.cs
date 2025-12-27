// ============================================
// Parental Presence App - Data Models
// C# Classes for SQLite Database
// ============================================

using SQLite;
using System;
using System.Collections.Generic;

namespace HereForYou.Models
{
    // ============================================
    // DetectionEvent - זיהוי של מילת מפתח
    // ============================================
    [Table("detection_events")]
    public class DetectionEvent
    {
        [PrimaryKey, AutoIncrement]
        [Column("id")]
        public int Id { get; set; }

        [Column("timestamp")]
        [Indexed]
        public DateTime Timestamp { get; set; }

        [Column("keyword")]
        [Indexed]
        public string Keyword { get; set; }

        [Column("confidence")]
        public float Confidence { get; set; }

        [Column("was_responded_to")]
        [Indexed]
        public bool WasRespondedTo { get; set; }

        [Column("response_time_seconds")]
        public int? ResponseTimeSeconds { get; set; }

        [Column("was_part_of_alert")]
        public bool WasPartOfAlert { get; set; }

        [Column("alert_id")]
        public int? AlertId { get; set; }

        [Column("context")]
        public string Context { get; set; }  // JSON string

        // Computed properties (not stored)
        [Ignore]
        public TimeSpan? ResponseTime => ResponseTimeSeconds.HasValue 
            ? TimeSpan.FromSeconds(ResponseTimeSeconds.Value) 
            : null;
    }

    // ============================================
    // Alert - התראה שנשלחה להורה
    // ============================================
    [Table("alerts")]
    public class Alert
    {
        [PrimaryKey, AutoIncrement]
        [Column("id")]
        public int Id { get; set; }

        [Column("created_at")]
        [Indexed]
        public DateTime CreatedAt { get; set; }

        [Column("keyword")]
        public string Keyword { get; set; }

        [Column("detection_count")]
        public int DetectionCount { get; set; }

        [Column("alert_level")]
        public string AlertLevel { get; set; }  // 'Info', 'Warning', 'Critical'

        [Column("was_acknowledged")]
        [Indexed]
        public bool WasAcknowledged { get; set; }

        [Column("acknowledged_at")]
        public DateTime? AcknowledgedAt { get; set; }

        [Column("acknowledgment_type")]
        public string AcknowledgmentType { get; set; }  // 'handled', 'snoozed', 'dismissed'

        [Column("time_window_seconds")]
        public int TimeWindowSeconds { get; set; } = 30;

        // Computed properties
        [Ignore]
        public TimeSpan? AcknowledgmentTime => WasAcknowledged && AcknowledgedAt.HasValue
            ? AcknowledgedAt.Value - CreatedAt
            : null;
    }

    // ============================================
    // ScreenTimeSession - סשן של זמן מסך
    // ============================================
    [Table("screen_time_sessions")]
    public class ScreenTimeSession
    {
        [PrimaryKey, AutoIncrement]
        [Column("id")]
        public int Id { get; set; }

        [Column("start_time")]
        [Indexed]
        public DateTime StartTime { get; set; }

        [Column("end_time")]
        public DateTime? EndTime { get; set; }

        [Column("duration_seconds")]
        public int? DurationSeconds { get; set; }

        [Column("app_name")]
        public string AppName { get; set; } = "General";

        [Column("was_interrupted")]
        public bool WasInterrupted { get; set; }

        [Column("interruption_count")]
        public int InterruptionCount { get; set; }

        [Column("device_type")]
        public string DeviceType { get; set; } = "Mobile";

        // Computed properties
        [Ignore]
        public TimeSpan Duration => EndTime.HasValue
            ? EndTime.Value - StartTime
            : DateTime.Now - StartTime;

        [Ignore]
        public bool IsActive => !EndTime.HasValue;
    }

    // ============================================
    // DailySummary - סיכום יומי
    // ============================================
    [Table("daily_summaries")]
    public class DailySummary
    {
        [PrimaryKey, AutoIncrement]
        [Column("id")]
        public int Id { get; set; }

        [Column("date")]
        [Indexed]
        [Unique]
        public DateTime Date { get; set; }

        [Column("total_screen_time_seconds")]
        public int TotalScreenTimeSeconds { get; set; }

        [Column("total_detections")]
        public int TotalDetections { get; set; }

        [Column("total_alerts")]
        public int TotalAlerts { get; set; }

        [Column("responded_detections")]
        public int RespondedDetections { get; set; }

        [Column("average_response_time_seconds")]
        public int? AverageResponseTimeSeconds { get; set; }

        [Column("presence_score")]
        public float? PresenceScore { get; set; }

        [Column("longest_screen_session_seconds")]
        public int? LongestScreenSessionSeconds { get; set; }

        [Column("created_at")]
        public DateTime CreatedAt { get; set; }

        [Column("updated_at")]
        public DateTime UpdatedAt { get; set; }

        // Computed properties
        [Ignore]
        public TimeSpan TotalScreenTime => TimeSpan.FromSeconds(TotalScreenTimeSeconds);

        [Ignore]
        public double ResponseRate => TotalDetections > 0
            ? (double)RespondedDetections / TotalDetections * 100
            : 0;
    }

    // ============================================
    // UserSetting - הגדרת משתמש
    // ============================================
    [Table("user_settings")]
    public class UserSetting
    {
        [PrimaryKey]
        [Column("key")]
        public string Key { get; set; }

        [Column("value")]
        public string Value { get; set; }

        [Column("value_type")]
        public string ValueType { get; set; }  // 'string', 'int', 'bool', 'json'

        [Column("updated_at")]
        public DateTime UpdatedAt { get; set; }
    }

    // ============================================
    // KeywordProfile - פרופיל מילת מפתח
    // ============================================
    [Table("keyword_profiles")]
    public class KeywordProfile
    {
        [PrimaryKey, AutoIncrement]
        [Column("id")]
        public int Id { get; set; }

        [Column("keyword")]
        [Unique]
        public string Keyword { get; set; }

        [Column("display_name")]
        public string DisplayName { get; set; }

        [Column("model_path")]
        public string ModelPath { get; set; }

        [Column("sensitivity")]
        public float Sensitivity { get; set; } = 0.7f;

        [Column("is_enabled")]
        public bool IsEnabled { get; set; } = true;

        [Column("voice_sample_path")]
        public string VoiceSamplePath { get; set; }

        [Column("created_at")]
        public DateTime CreatedAt { get; set; }

        [Column("updated_at")]
        public DateTime UpdatedAt { get; set; }
    }

    // ============================================
    // AnalyticsEvent - אירוע אנליטיקס
    // ============================================
    [Table("analytics_events")]
    public class AnalyticsEvent
    {
        [PrimaryKey, AutoIncrement]
        [Column("id")]
        public int Id { get; set; }

        [Column("event_type")]
        [Indexed]
        public string EventType { get; set; }

        [Column("event_data")]
        public string EventData { get; set; }  // JSON string

        [Column("timestamp")]
        [Indexed]
        public DateTime Timestamp { get; set; }
    }

    // ============================================
    // SchemaVersion - גרסת סכימה
    // ============================================
    [Table("schema_version")]
    public class SchemaVersion
    {
        [PrimaryKey]
        [Column("version")]
        public int Version { get; set; }

        [Column("applied_at")]
        public DateTime AppliedAt { get; set; }

        [Column("description")]
        public string Description { get; set; }
    }

    // ============================================
    // DTOs and View Models
    // ============================================

    /// <summary>
    /// סטטיסטיקות יומיות - מחושבות מה-View
    /// </summary>
    public class DailyStats
    {
        public DateTime Date { get; set; }
        public int TotalSessions { get; set; }
        public int TotalScreenTimeSeconds { get; set; }
        public int AvgSessionDurationSeconds { get; set; }
        public int LongestSessionSeconds { get; set; }
        public int InterruptedSessions { get; set; }

        public TimeSpan TotalScreenTime => TimeSpan.FromSeconds(TotalScreenTimeSeconds);
        public TimeSpan AvgSessionDuration => TimeSpan.FromSeconds(AvgSessionDurationSeconds);
    }

    /// <summary>
    /// סטטיסטיקות זיהוי - מחושבות מה-View
    /// </summary>
    public class DetectionStats
    {
        public DateTime Date { get; set; }
        public string Keyword { get; set; }
        public int TotalDetections { get; set; }
        public int RespondedCount { get; set; }
        public int? AvgResponseTimeSeconds { get; set; }
        public int? FastestResponseSeconds { get; set; }
        public int? SlowestResponseSeconds { get; set; }

        public double ResponseRate => TotalDetections > 0
            ? (double)RespondedCount / TotalDetections * 100
            : 0;
    }

    /// <summary>
    /// סיכום התראות - מחושב מה-View
    /// </summary>
    public class AlertSummary
    {
        public DateTime Date { get; set; }
        public string AlertLevel { get; set; }
        public int TotalAlerts { get; set; }
        public int AcknowledgedCount { get; set; }
        public int? AvgAcknowledgmentTimeSeconds { get; set; }

        public double AcknowledgmentRate => TotalAlerts > 0
            ? (double)AcknowledgedCount / TotalAlerts * 100
            : 0;
    }

    // ============================================
    // Enums
    // ============================================

    public enum AlertLevel
    {
        Info,
        Warning,
        Critical
    }

    public enum AcknowledgmentType
    {
        Handled,
        Snoozed,
        Dismissed
    }

    public enum DeviceType
    {
        Mobile,
        Desktop,
        Tablet
    }

    public enum EventType
    {
        AppOpened,
        AppClosed,
        MonitoringStarted,
        MonitoringStopped,
        SettingsChanged,
        AlertShown,
        AlertAcknowledged
    }
}
