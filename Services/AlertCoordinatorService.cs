using HereForYou.Models;
using HereForYou.Services.Interfaces;

namespace HereForYou.Services;

/// <summary>
/// Coordinates detection events and triggers alerts when threshold is reached.
/// </summary>
public class AlertCoordinatorService : IAlertCoordinatorService
{
    private readonly IDatabaseService _database;
    private readonly ISettingsService _settings;
    private readonly INotificationService _notifications;
    private readonly Queue<DetectionEvent> _pendingDetections = new();
    private readonly object _lock = new();
    private System.Timers.Timer? _cleanupTimer;

    /// <inheritdoc/>
    public int PendingDetectionCount
    {
        get
        {
            lock (_lock)
            {
                return _pendingDetections.Count;
            }
        }
    }

    /// <inheritdoc/>
    public event EventHandler<AlertTriggeredEventArgs>? AlertTriggered;

    /// <summary>
    /// Initializes a new instance of the <see cref="AlertCoordinatorService"/> class.
    /// </summary>
    public AlertCoordinatorService(
        IDatabaseService database,
        ISettingsService settings,
        INotificationService notifications)
    {
        _database = database;
        _settings = settings;
        _notifications = notifications;
    }

    /// <inheritdoc/>
    public async Task InitializeAsync()
    {
        await _settings.InitializeAsync().ConfigureAwait(false);

        // Start cleanup timer to remove old detections
        _cleanupTimer = new System.Timers.Timer(5000); // Every 5 seconds
        _cleanupTimer.Elapsed += (s, e) => CleanupOldDetections();
        _cleanupTimer.Start();
    }

    /// <inheritdoc/>
    public async Task ProcessDetectionAsync(DetectionEvent detection)
    {
        // Save to database
        await _database.SaveDetectionEventAsync(detection).ConfigureAwait(false);

        // Log analytics
        await _database.LogAnalyticsEventAsync(
            EventTypes.KeywordDetected,
            System.Text.Json.JsonSerializer.Serialize(new { detection.Keyword, detection.Confidence }))
            .ConfigureAwait(false);

        // Add to pending queue
        lock (_lock)
        {
            _pendingDetections.Enqueue(detection);
            CleanupOldDetections();
        }

        // Check if we should trigger an alert
        await CheckAndTriggerAlertAsync().ConfigureAwait(false);
    }

    private async Task CheckAndTriggerAlertAsync()
    {
        List<DetectionEvent> detectionsToAlert;

        lock (_lock)
        {
            if (_pendingDetections.Count < _settings.DetectionThreshold)
            {
                return;
            }

            // Get all pending detections
            detectionsToAlert = _pendingDetections.ToList();
        }

        // Create alert
        var alert = new Alert
        {
            CreatedAt = DateTime.Now,
            Keyword = detectionsToAlert.Last().Keyword,
            DetectionCount = detectionsToAlert.Count,
            AlertLevel = DetermineAlertLevel(detectionsToAlert.Count),
            TimeWindowSeconds = _settings.ThresholdWindowSeconds
        };

        // Save alert
        await _database.SaveAlertAsync(alert).ConfigureAwait(false);

        // Update detections with alert ID
        foreach (var detection in detectionsToAlert)
        {
            detection.WasPartOfAlert = true;
            detection.AlertId = alert.Id;
            await _database.UpdateDetectionEventAsync(detection).ConfigureAwait(false);
        }

        // Clear pending detections
        ClearPendingDetections();

        // Raise event
        AlertTriggered?.Invoke(this, new AlertTriggeredEventArgs(alert, detectionsToAlert));

        // Show notification
        await ShowAlertNotificationAsync(alert).ConfigureAwait(false);

        // Log analytics
        await _database.LogAnalyticsEventAsync(
            EventTypes.AlertShown,
            System.Text.Json.JsonSerializer.Serialize(new { alert.AlertLevel, alert.DetectionCount }))
            .ConfigureAwait(false);
    }

    private string DetermineAlertLevel(int detectionCount)
    {
        return detectionCount switch
        {
            >= 5 => nameof(AlertLevelType.Critical),
            >= 3 => nameof(AlertLevelType.Warning),
            _ => nameof(AlertLevelType.Info)
        };
    }

    private async Task ShowAlertNotificationAsync(Alert alert)
    {
        string title = $"הילד שלך קורא לך!";
        string message = $"\"{alert.Keyword}\" נאמר {alert.DetectionCount} פעמים";

        var level = Enum.Parse<AlertLevelType>(alert.AlertLevel);

        if (level == AlertLevelType.Critical && _settings.EnableOverlayAlerts)
        {
            await _notifications.ShowOverlayAlertAsync(title, message, alert.Id).ConfigureAwait(false);
        }
        else
        {
            await _notifications.ShowAlertAsync(title, message, level, alert.Id).ConfigureAwait(false);
        }

        if (_settings.EnableSoundAlerts)
        {
            await _notifications.PlayAlertSoundAsync(level).ConfigureAwait(false);
        }

        if (_settings.EnableVibration)
        {
            await _notifications.VibrateAsync(level).ConfigureAwait(false);
        }
    }

    /// <inheritdoc/>
    public async Task HandleAlertResponseAsync(int alertId, AcknowledgmentType response)
    {
        var alert = await _database.GetAlertByIdAsync(alertId).ConfigureAwait(false);
        if (alert == null)
        {
            return;
        }

        alert.WasAcknowledged = true;
        alert.AcknowledgedAt = DateTime.Now;
        alert.AcknowledgmentType = response.ToString();

        await _database.UpdateAlertAsync(alert).ConfigureAwait(false);

        // Update related detections
        var detections = await _database.GetDetectionEventsAsync(
            alert.CreatedAt.AddSeconds(-alert.TimeWindowSeconds),
            alert.CreatedAt).ConfigureAwait(false);

        foreach (var detection in detections.Where(d => d.AlertId == alertId))
        {
            detection.WasRespondedTo = response == AcknowledgmentType.Handled;
            if (detection.WasRespondedTo && alert.AcknowledgedAt.HasValue)
            {
                detection.ResponseTimeSeconds = (int)(alert.AcknowledgedAt.Value - alert.CreatedAt).TotalSeconds;
            }

            await _database.UpdateDetectionEventAsync(detection).ConfigureAwait(false);
        }

        // Log analytics
        await _database.LogAnalyticsEventAsync(
            EventTypes.AlertAcknowledged,
            System.Text.Json.JsonSerializer.Serialize(new { response = response.ToString() }))
            .ConfigureAwait(false);

        // Dismiss overlay if shown
        await _notifications.DismissOverlayAsync().ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public void ClearPendingDetections()
    {
        lock (_lock)
        {
            _pendingDetections.Clear();
        }
    }

    private void CleanupOldDetections()
    {
        var cutoff = DateTime.Now.AddSeconds(-_settings.ThresholdWindowSeconds);

        lock (_lock)
        {
            while (_pendingDetections.Count > 0 && _pendingDetections.Peek().Timestamp < cutoff)
            {
                _pendingDetections.Dequeue();
            }
        }
    }
}
