using System.Text.Json;

using HereForYou.Models;
using HereForYou.Services.Interfaces;

using SQLite;

namespace HereForYou.Services;

/// <summary>
/// SQLite database service implementation.
/// </summary>
public class DatabaseService : IDatabaseService
{
    private readonly SQLiteAsyncConnection _database;
    private bool _isInitialized;

    /// <summary>
    /// Initializes a new instance of the <see cref="DatabaseService"/> class.
    /// </summary>
    /// <param name="dbPath">Path to the SQLite database file.</param>
    public DatabaseService(string dbPath)
    {
        _database = new SQLiteAsyncConnection(dbPath, SQLiteOpenFlags.Create | SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.SharedCache);
    }

    /// <inheritdoc/>
    public async Task InitializeAsync()
    {
        if (_isInitialized)
        {
            return;
        }

        // Create tables
        await _database.CreateTableAsync<DetectionEvent>().ConfigureAwait(false);
        await _database.CreateTableAsync<Alert>().ConfigureAwait(false);
        await _database.CreateTableAsync<ScreenTimeSession>().ConfigureAwait(false);
        await _database.CreateTableAsync<DailySummary>().ConfigureAwait(false);
        await _database.CreateTableAsync<UserSetting>().ConfigureAwait(false);
        await _database.CreateTableAsync<KeywordProfile>().ConfigureAwait(false);
        await _database.CreateTableAsync<AnalyticsEvent>().ConfigureAwait(false);

        // Initialize default settings
        await InitializeDefaultSettingsAsync().ConfigureAwait(false);

        // Initialize default keyword profiles
        await InitializeDefaultKeywordProfilesAsync().ConfigureAwait(false);

        _isInitialized = true;
    }

    private async Task InitializeDefaultSettingsAsync()
    {
        var existingSettings = await _database.Table<UserSetting>().CountAsync().ConfigureAwait(false);
        if (existingSettings > 0)
        {
            return;
        }

        var defaults = new List<UserSetting>
        {
            new() { Key = SettingKeys.Keywords, Value = "[\"אמא\",\"אבא\"]", ValueType = "json", UpdatedAt = DateTime.Now },
            new() { Key = SettingKeys.DetectionThreshold, Value = "3", ValueType = "int", UpdatedAt = DateTime.Now },
            new() { Key = SettingKeys.ThresholdWindowSeconds, Value = "30", ValueType = "int", UpdatedAt = DateTime.Now },
            new() { Key = SettingKeys.IsMonitoringEnabled, Value = "false", ValueType = "bool", UpdatedAt = DateTime.Now },
            new() { Key = SettingKeys.AlertLevelDefault, Value = "Warning", ValueType = "string", UpdatedAt = DateTime.Now },
            new() { Key = SettingKeys.EnableOverlayAlerts, Value = "true", ValueType = "bool", UpdatedAt = DateTime.Now },
            new() { Key = SettingKeys.EnableSoundAlerts, Value = "true", ValueType = "bool", UpdatedAt = DateTime.Now },
            new() { Key = SettingKeys.EnableVibration, Value = "true", ValueType = "bool", UpdatedAt = DateTime.Now },
            new() { Key = SettingKeys.ConfidenceThreshold, Value = "0.7", ValueType = "float", UpdatedAt = DateTime.Now },
            new() { Key = SettingKeys.UserName, Value = "", ValueType = "string", UpdatedAt = DateTime.Now },
            new() { Key = SettingKeys.FamilyMemberNames, Value = "[]", ValueType = "json", UpdatedAt = DateTime.Now },
        };

        await _database.InsertAllAsync(defaults).ConfigureAwait(false);
    }

    private async Task InitializeDefaultKeywordProfilesAsync()
    {
        var existingProfiles = await _database.Table<KeywordProfile>().CountAsync().ConfigureAwait(false);
        if (existingProfiles > 0)
        {
            return;
        }

        var defaults = new List<KeywordProfile>
        {
            new()
            {
                Keyword = "ima",
                DisplayName = "אמא",
                Sensitivity = 0.7f,
                IsEnabled = true,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now
            },
            new()
            {
                Keyword = "abba",
                DisplayName = "אבא",
                Sensitivity = 0.7f,
                IsEnabled = true,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now
            }
        };

        await _database.InsertAllAsync(defaults).ConfigureAwait(false);
    }

    #region Detection Events

    /// <inheritdoc/>
    public async Task<int> SaveDetectionEventAsync(DetectionEvent detectionEvent)
    {
        await EnsureInitializedAsync().ConfigureAwait(false);

        if (detectionEvent.Id == 0)
        {
            return await _database.InsertAsync(detectionEvent).ConfigureAwait(false);
        }

        return await _database.UpdateAsync(detectionEvent).ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public async Task<DetectionEvent?> GetDetectionEventByIdAsync(int id)
    {
        await EnsureInitializedAsync().ConfigureAwait(false);
        return await _database.Table<DetectionEvent>()
            .Where(e => e.Id == id)
            .FirstOrDefaultAsync()
            .ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public async Task<List<DetectionEvent>> GetDetectionEventsAsync(DateTime from, DateTime to)
    {
        await EnsureInitializedAsync().ConfigureAwait(false);
        return await _database.Table<DetectionEvent>()
            .Where(e => e.Timestamp >= from && e.Timestamp <= to)
            .OrderByDescending(e => e.Timestamp)
            .ToListAsync()
            .ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public async Task<List<DetectionEvent>> GetRecentDetectionEventsAsync(int count)
    {
        await EnsureInitializedAsync().ConfigureAwait(false);
        return await _database.Table<DetectionEvent>()
            .OrderByDescending(e => e.Timestamp)
            .Take(count)
            .ToListAsync()
            .ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public async Task<List<DetectionEvent>> GetDetectionEventsByKeywordAsync(string keyword, DateTime? from = null, DateTime? to = null)
    {
        await EnsureInitializedAsync().ConfigureAwait(false);

        var query = _database.Table<DetectionEvent>().Where(e => e.Keyword == keyword);

        if (from.HasValue)
        {
            query = query.Where(e => e.Timestamp >= from.Value);
        }

        if (to.HasValue)
        {
            query = query.Where(e => e.Timestamp <= to.Value);
        }

        return await query.OrderByDescending(e => e.Timestamp).ToListAsync().ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public async Task<int> UpdateDetectionEventAsync(DetectionEvent detectionEvent)
    {
        await EnsureInitializedAsync().ConfigureAwait(false);
        return await _database.UpdateAsync(detectionEvent).ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public async Task<int> DeleteDetectionEventAsync(int id)
    {
        await EnsureInitializedAsync().ConfigureAwait(false);
        return await _database.DeleteAsync<DetectionEvent>(id).ConfigureAwait(false);
    }

    #endregion

    #region Alerts

    /// <inheritdoc/>
    public async Task<int> SaveAlertAsync(Alert alert)
    {
        await EnsureInitializedAsync().ConfigureAwait(false);

        if (alert.Id == 0)
        {
            return await _database.InsertAsync(alert).ConfigureAwait(false);
        }

        return await _database.UpdateAsync(alert).ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public async Task<Alert?> GetAlertByIdAsync(int id)
    {
        await EnsureInitializedAsync().ConfigureAwait(false);
        return await _database.Table<Alert>()
            .Where(a => a.Id == id)
            .FirstOrDefaultAsync()
            .ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public async Task<List<Alert>> GetAlertsAsync(DateTime from, DateTime to)
    {
        await EnsureInitializedAsync().ConfigureAwait(false);
        return await _database.Table<Alert>()
            .Where(a => a.CreatedAt >= from && a.CreatedAt <= to)
            .OrderByDescending(a => a.CreatedAt)
            .ToListAsync()
            .ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public async Task<List<Alert>> GetUnacknowledgedAlertsAsync()
    {
        await EnsureInitializedAsync().ConfigureAwait(false);
        return await _database.Table<Alert>()
            .Where(a => !a.WasAcknowledged)
            .OrderByDescending(a => a.CreatedAt)
            .ToListAsync()
            .ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public async Task<int> UpdateAlertAsync(Alert alert)
    {
        await EnsureInitializedAsync().ConfigureAwait(false);
        return await _database.UpdateAsync(alert).ConfigureAwait(false);
    }

    #endregion

    #region Screen Time Sessions

    /// <inheritdoc/>
    public async Task<int> SaveScreenTimeSessionAsync(ScreenTimeSession session)
    {
        await EnsureInitializedAsync().ConfigureAwait(false);

        if (session.Id == 0)
        {
            return await _database.InsertAsync(session).ConfigureAwait(false);
        }

        return await _database.UpdateAsync(session).ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public async Task<List<ScreenTimeSession>> GetScreenTimeSessionsAsync(DateTime date)
    {
        await EnsureInitializedAsync().ConfigureAwait(false);

        var startOfDay = date.Date;
        var endOfDay = startOfDay.AddDays(1);

        return await _database.Table<ScreenTimeSession>()
            .Where(s => s.StartTime >= startOfDay && s.StartTime < endOfDay)
            .OrderByDescending(s => s.StartTime)
            .ToListAsync()
            .ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public async Task<ScreenTimeSession?> GetActiveSessionAsync()
    {
        await EnsureInitializedAsync().ConfigureAwait(false);
        return await _database.Table<ScreenTimeSession>()
            .Where(s => s.EndTime == null)
            .OrderByDescending(s => s.StartTime)
            .FirstOrDefaultAsync()
            .ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public async Task<int> UpdateScreenTimeSessionAsync(ScreenTimeSession session)
    {
        await EnsureInitializedAsync().ConfigureAwait(false);
        return await _database.UpdateAsync(session).ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public async Task<TimeSpan> GetTotalScreenTimeAsync(DateTime date)
    {
        await EnsureInitializedAsync().ConfigureAwait(false);

        var sessions = await GetScreenTimeSessionsAsync(date).ConfigureAwait(false);
        var totalSeconds = sessions.Sum(s => s.DurationSeconds ?? (int)s.Duration.TotalSeconds);

        return TimeSpan.FromSeconds(totalSeconds);
    }

    #endregion

    #region Daily Summaries

    /// <inheritdoc/>
    public async Task<DailySummary> GetOrCreateDailySummaryAsync(DateTime date)
    {
        await EnsureInitializedAsync().ConfigureAwait(false);

        var dateOnly = date.Date;
        var existing = await _database.Table<DailySummary>()
            .Where(s => s.Date == dateOnly)
            .FirstOrDefaultAsync()
            .ConfigureAwait(false);

        if (existing != null)
        {
            return existing;
        }

        var newSummary = new DailySummary
        {
            Date = dateOnly,
            CreatedAt = DateTime.Now,
            UpdatedAt = DateTime.Now
        };

        await _database.InsertAsync(newSummary).ConfigureAwait(false);
        return newSummary;
    }

    /// <inheritdoc/>
    public async Task<int> UpdateDailySummaryAsync(DailySummary summary)
    {
        await EnsureInitializedAsync().ConfigureAwait(false);
        summary.UpdatedAt = DateTime.Now;
        return await _database.UpdateAsync(summary).ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public async Task<List<DailySummary>> GetDailySummariesAsync(DateTime from, DateTime to)
    {
        await EnsureInitializedAsync().ConfigureAwait(false);
        return await _database.Table<DailySummary>()
            .Where(s => s.Date >= from.Date && s.Date <= to.Date)
            .OrderByDescending(s => s.Date)
            .ToListAsync()
            .ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public async Task RecalculateDailySummaryAsync(DateTime date)
    {
        await EnsureInitializedAsync().ConfigureAwait(false);

        var summary = await GetOrCreateDailySummaryAsync(date).ConfigureAwait(false);

        var startOfDay = date.Date;
        var endOfDay = startOfDay.AddDays(1);

        // Get detections for the day
        var detections = await GetDetectionEventsAsync(startOfDay, endOfDay).ConfigureAwait(false);
        summary.TotalDetections = detections.Count;
        summary.RespondedDetections = detections.Count(d => d.WasRespondedTo);

        if (detections.Any(d => d.ResponseTimeSeconds.HasValue))
        {
            summary.AverageResponseTimeSeconds = (int)detections
                .Where(d => d.ResponseTimeSeconds.HasValue)
                .Average(d => d.ResponseTimeSeconds!.Value);
        }

        // Get alerts for the day
        var alerts = await GetAlertsAsync(startOfDay, endOfDay).ConfigureAwait(false);
        summary.TotalAlerts = alerts.Count;

        // Get screen time for the day
        var screenTime = await GetTotalScreenTimeAsync(date).ConfigureAwait(false);
        summary.TotalScreenTimeSeconds = (int)screenTime.TotalSeconds;

        // Get longest session
        var sessions = await GetScreenTimeSessionsAsync(date).ConfigureAwait(false);
        if (sessions.Count != 0)
        {
            summary.LongestScreenSessionSeconds = sessions.Max(s => s.DurationSeconds ?? (int)s.Duration.TotalSeconds);
        }

        // Calculate presence score
        summary.PresenceScore = CalculatePresenceScore(summary);

        await UpdateDailySummaryAsync(summary).ConfigureAwait(false);
    }

    private static float CalculatePresenceScore(DailySummary summary)
    {
        // Base score of 100
        float score = 100f;

        // Deduct 10 points for each hour of screen time over 4 hours
        float excessHours = Math.Max(0, (summary.TotalScreenTimeSeconds / 3600f) - 4f);
        score -= excessHours * 10f;

        // Deduct 5 points for each unresponded detection
        int unrespondedDetections = summary.TotalDetections - summary.RespondedDetections;
        score -= unrespondedDetections * 5f;

        // Bonus for high response rate
        if (summary.TotalDetections > 0)
        {
            float responseRate = (float)summary.RespondedDetections / summary.TotalDetections;
            if (responseRate >= 0.9f)
            {
                score += 5f;
            }
        }

        return Math.Clamp(score, 0f, 100f);
    }

    #endregion

    #region User Settings

    /// <inheritdoc/>
    public async Task<T?> GetSettingAsync<T>(string key)
    {
        await EnsureInitializedAsync().ConfigureAwait(false);

        var setting = await _database.Table<UserSetting>()
            .Where(s => s.Key == key)
            .FirstOrDefaultAsync()
            .ConfigureAwait(false);

        if (setting == null)
        {
            return default;
        }

        return ConvertSettingValue<T>(setting.Value, setting.ValueType);
    }

    /// <inheritdoc/>
    public async Task SetSettingAsync<T>(string key, T value)
    {
        await EnsureInitializedAsync().ConfigureAwait(false);

        string valueType = GetValueType<T>();
        string stringValue = ConvertToString(value, valueType);

        var existing = await _database.Table<UserSetting>()
            .Where(s => s.Key == key)
            .FirstOrDefaultAsync()
            .ConfigureAwait(false);

        if (existing != null)
        {
            existing.Value = stringValue;
            existing.ValueType = valueType;
            existing.UpdatedAt = DateTime.Now;
            await _database.UpdateAsync(existing).ConfigureAwait(false);
        }
        else
        {
            var newSetting = new UserSetting
            {
                Key = key,
                Value = stringValue,
                ValueType = valueType,
                UpdatedAt = DateTime.Now
            };
            await _database.InsertAsync(newSetting).ConfigureAwait(false);
        }
    }

    /// <inheritdoc/>
    public async Task<List<UserSetting>> GetAllSettingsAsync()
    {
        await EnsureInitializedAsync().ConfigureAwait(false);
        return await _database.Table<UserSetting>().ToListAsync().ConfigureAwait(false);
    }

    private static T? ConvertSettingValue<T>(string value, string valueType)
    {
        if (string.IsNullOrEmpty(value))
        {
            return default;
        }

        return valueType switch
        {
            "int" => (T)(object)int.Parse(value),
            "float" => (T)(object)float.Parse(value),
            "bool" => (T)(object)bool.Parse(value),
            "json" => JsonSerializer.Deserialize<T>(value),
            _ => (T)(object)value
        };
    }

    private static string GetValueType<T>()
    {
        var type = typeof(T);

        if (type == typeof(int))
        {
            return "int";
        }

        if (type == typeof(float))
        {
            return "float";
        }

        if (type == typeof(bool))
        {
            return "bool";
        }

        if (type == typeof(List<string>) || type.IsGenericType)
        {
            return "json";
        }

        return "string";
    }

    private static string ConvertToString<T>(T value, string valueType)
    {
        if (value == null)
        {
            return string.Empty;
        }

        return valueType switch
        {
            "json" => JsonSerializer.Serialize(value),
            _ => value.ToString() ?? string.Empty
        };
    }

    #endregion

    #region Keyword Profiles

    /// <inheritdoc/>
    public async Task<List<KeywordProfile>> GetKeywordProfilesAsync()
    {
        await EnsureInitializedAsync().ConfigureAwait(false);
        return await _database.Table<KeywordProfile>()
            .OrderBy(p => p.DisplayName)
            .ToListAsync()
            .ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public async Task<List<KeywordProfile>> GetEnabledKeywordProfilesAsync()
    {
        await EnsureInitializedAsync().ConfigureAwait(false);
        return await _database.Table<KeywordProfile>()
            .Where(p => p.IsEnabled)
            .OrderBy(p => p.DisplayName)
            .ToListAsync()
            .ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public async Task<int> SaveKeywordProfileAsync(KeywordProfile profile)
    {
        await EnsureInitializedAsync().ConfigureAwait(false);
        profile.UpdatedAt = DateTime.Now;

        if (profile.Id == 0)
        {
            profile.CreatedAt = DateTime.Now;
            return await _database.InsertAsync(profile).ConfigureAwait(false);
        }

        return await _database.UpdateAsync(profile).ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public async Task<int> DeleteKeywordProfileAsync(int id)
    {
        await EnsureInitializedAsync().ConfigureAwait(false);
        return await _database.DeleteAsync<KeywordProfile>(id).ConfigureAwait(false);
    }

    #endregion

    #region Analytics

    /// <inheritdoc/>
    public async Task LogAnalyticsEventAsync(string eventType, string? eventData = null)
    {
        await EnsureInitializedAsync().ConfigureAwait(false);

        var analyticsEvent = new AnalyticsEvent
        {
            EventType = eventType,
            EventData = eventData,
            Timestamp = DateTime.Now
        };

        await _database.InsertAsync(analyticsEvent).ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public async Task<List<AnalyticsEvent>> GetAnalyticsEventsAsync(DateTime from, DateTime to, string? eventType = null)
    {
        await EnsureInitializedAsync().ConfigureAwait(false);

        var query = _database.Table<AnalyticsEvent>()
            .Where(e => e.Timestamp >= from && e.Timestamp <= to);

        if (!string.IsNullOrEmpty(eventType))
        {
            query = query.Where(e => e.EventType == eventType);
        }

        return await query.OrderByDescending(e => e.Timestamp).ToListAsync().ConfigureAwait(false);
    }

    #endregion

    #region Maintenance

    /// <inheritdoc/>
    public async Task CleanupOldDataAsync(int daysToKeep = 90)
    {
        await EnsureInitializedAsync().ConfigureAwait(false);

        var cutoffDate = DateTime.Now.AddDays(-daysToKeep);

        await _database.ExecuteAsync(
            "DELETE FROM detection_events WHERE timestamp < ?",
            cutoffDate).ConfigureAwait(false);

        await _database.ExecuteAsync(
            "DELETE FROM alerts WHERE created_at < ?",
            cutoffDate).ConfigureAwait(false);

        await _database.ExecuteAsync(
            "DELETE FROM screen_time_sessions WHERE start_time < ?",
            cutoffDate).ConfigureAwait(false);

        await _database.ExecuteAsync(
            "DELETE FROM analytics_events WHERE timestamp < ?",
            cutoffDate).ConfigureAwait(false);

        // Keep daily summaries for a year
        var summariesCutoff = DateTime.Now.AddDays(-365);
        await _database.ExecuteAsync(
            "DELETE FROM daily_summaries WHERE date < ?",
            summariesCutoff).ConfigureAwait(false);
    }

    #endregion

    private async Task EnsureInitializedAsync()
    {
        if (!_isInitialized)
        {
            await InitializeAsync().ConfigureAwait(false);
        }
    }
}
