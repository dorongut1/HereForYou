using HereForYou.Models;

namespace HereForYou.Services.Interfaces;

/// <summary>
/// Interface for database operations.
/// </summary>
public interface IDatabaseService
{
    /// <summary>
    /// Initializes the database and creates tables.
    /// </summary>
    Task InitializeAsync();

    // Detection Events
    /// <summary>
    /// Saves a detection event to the database.
    /// </summary>
    Task<int> SaveDetectionEventAsync(DetectionEvent detectionEvent);

    /// <summary>
    /// Gets a detection event by ID.
    /// </summary>
    Task<DetectionEvent?> GetDetectionEventByIdAsync(int id);

    /// <summary>
    /// Gets detection events within a date range.
    /// </summary>
    Task<List<DetectionEvent>> GetDetectionEventsAsync(DateTime from, DateTime to);

    /// <summary>
    /// Gets recent detection events.
    /// </summary>
    Task<List<DetectionEvent>> GetRecentDetectionEventsAsync(int count);

    /// <summary>
    /// Gets detection events by keyword.
    /// </summary>
    Task<List<DetectionEvent>> GetDetectionEventsByKeywordAsync(string keyword, DateTime? from = null, DateTime? to = null);

    /// <summary>
    /// Updates a detection event.
    /// </summary>
    Task<int> UpdateDetectionEventAsync(DetectionEvent detectionEvent);

    /// <summary>
    /// Deletes a detection event.
    /// </summary>
    Task<int> DeleteDetectionEventAsync(int id);

    // Alerts
    /// <summary>
    /// Saves an alert to the database.
    /// </summary>
    Task<int> SaveAlertAsync(Alert alert);

    /// <summary>
    /// Gets an alert by ID.
    /// </summary>
    Task<Alert?> GetAlertByIdAsync(int id);

    /// <summary>
    /// Gets alerts within a date range.
    /// </summary>
    Task<List<Alert>> GetAlertsAsync(DateTime from, DateTime to);

    /// <summary>
    /// Gets unacknowledged alerts.
    /// </summary>
    Task<List<Alert>> GetUnacknowledgedAlertsAsync();

    /// <summary>
    /// Updates an alert.
    /// </summary>
    Task<int> UpdateAlertAsync(Alert alert);

    // Screen Time Sessions
    /// <summary>
    /// Saves a screen time session.
    /// </summary>
    Task<int> SaveScreenTimeSessionAsync(ScreenTimeSession session);

    /// <summary>
    /// Gets screen time sessions for a specific date.
    /// </summary>
    Task<List<ScreenTimeSession>> GetScreenTimeSessionsAsync(DateTime date);

    /// <summary>
    /// Gets the current active session.
    /// </summary>
    Task<ScreenTimeSession?> GetActiveSessionAsync();

    /// <summary>
    /// Updates a screen time session.
    /// </summary>
    Task<int> UpdateScreenTimeSessionAsync(ScreenTimeSession session);

    /// <summary>
    /// Gets total screen time for a date.
    /// </summary>
    Task<TimeSpan> GetTotalScreenTimeAsync(DateTime date);

    // Daily Summaries
    /// <summary>
    /// Gets or creates a daily summary for the specified date.
    /// </summary>
    Task<DailySummary> GetOrCreateDailySummaryAsync(DateTime date);

    /// <summary>
    /// Updates a daily summary.
    /// </summary>
    Task<int> UpdateDailySummaryAsync(DailySummary summary);

    /// <summary>
    /// Gets daily summaries for a date range.
    /// </summary>
    Task<List<DailySummary>> GetDailySummariesAsync(DateTime from, DateTime to);

    /// <summary>
    /// Recalculates the daily summary for a date.
    /// </summary>
    Task RecalculateDailySummaryAsync(DateTime date);

    // User Settings
    /// <summary>
    /// Gets a setting value.
    /// </summary>
    Task<T?> GetSettingAsync<T>(string key);

    /// <summary>
    /// Sets a setting value.
    /// </summary>
    Task SetSettingAsync<T>(string key, T value);

    /// <summary>
    /// Gets all settings.
    /// </summary>
    Task<List<UserSetting>> GetAllSettingsAsync();

    // Keyword Profiles
    /// <summary>
    /// Gets all keyword profiles.
    /// </summary>
    Task<List<KeywordProfile>> GetKeywordProfilesAsync();

    /// <summary>
    /// Gets enabled keyword profiles.
    /// </summary>
    Task<List<KeywordProfile>> GetEnabledKeywordProfilesAsync();

    /// <summary>
    /// Saves a keyword profile.
    /// </summary>
    Task<int> SaveKeywordProfileAsync(KeywordProfile profile);

    /// <summary>
    /// Deletes a keyword profile.
    /// </summary>
    Task<int> DeleteKeywordProfileAsync(int id);

    // Analytics
    /// <summary>
    /// Logs an analytics event.
    /// </summary>
    Task LogAnalyticsEventAsync(string eventType, string? eventData = null);

    /// <summary>
    /// Gets analytics events.
    /// </summary>
    Task<List<AnalyticsEvent>> GetAnalyticsEventsAsync(DateTime from, DateTime to, string? eventType = null);

    // Maintenance
    /// <summary>
    /// Cleans up old data older than the specified number of days.
    /// </summary>
    Task CleanupOldDataAsync(int daysToKeep = 90);
}
