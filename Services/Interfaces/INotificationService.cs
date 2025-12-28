using HereForYou.Models;

namespace HereForYou.Services.Interfaces;

/// <summary>
/// Interface for notification services.
/// </summary>
public interface INotificationService
{
    /// <summary>
    /// Event raised when user responds to an alert.
    /// </summary>
    event EventHandler<AlertResponseEventArgs>? AlertResponded;

    /// <summary>
    /// Shows an alert notification.
    /// </summary>
    /// <param name="title">The alert title.</param>
    /// <param name="message">The alert message.</param>
    /// <param name="level">The alert severity level.</param>
    /// <param name="alertId">The associated alert ID.</param>
    Task ShowAlertAsync(string title, string message, AlertLevelType level, int? alertId = null);

    /// <summary>
    /// Shows a full-screen overlay alert (for critical alerts).
    /// </summary>
    /// <param name="title">The alert title.</param>
    /// <param name="message">The alert message.</param>
    /// <param name="alertId">The associated alert ID.</param>
    Task ShowOverlayAlertAsync(string title, string message, int? alertId = null);

    /// <summary>
    /// Dismisses any active overlay alert.
    /// </summary>
    Task DismissOverlayAsync();

    /// <summary>
    /// Shows a simple notification (non-blocking).
    /// </summary>
    /// <param name="title">The notification title.</param>
    /// <param name="message">The notification message.</param>
    Task ShowNotificationAsync(string title, string message);

    /// <summary>
    /// Shows the monitoring active notification (persistent).
    /// </summary>
    Task ShowMonitoringNotificationAsync();

    /// <summary>
    /// Hides the monitoring notification.
    /// </summary>
    Task HideMonitoringNotificationAsync();

    /// <summary>
    /// Checks if the app has notification permission.
    /// </summary>
    Task<bool> HasNotificationPermissionAsync();

    /// <summary>
    /// Requests notification permission.
    /// </summary>
    Task<bool> RequestNotificationPermissionAsync();

    /// <summary>
    /// Checks if the app has overlay permission (Android).
    /// </summary>
    Task<bool> HasOverlayPermissionAsync();

    /// <summary>
    /// Requests overlay permission (Android).
    /// </summary>
    Task<bool> RequestOverlayPermissionAsync();

    /// <summary>
    /// Plays an alert sound.
    /// </summary>
    Task PlayAlertSoundAsync(AlertLevelType level);

    /// <summary>
    /// Vibrates the device.
    /// </summary>
    Task VibrateAsync(AlertLevelType level);
}

/// <summary>
/// Event arguments for alert responses.
/// </summary>
public class AlertResponseEventArgs : EventArgs
{
    /// <summary>
    /// Gets the alert ID.
    /// </summary>
    public int? AlertId { get; }

    /// <summary>
    /// Gets the response type.
    /// </summary>
    public AcknowledgmentType ResponseType { get; }

    /// <summary>
    /// Gets the response timestamp.
    /// </summary>
    public DateTime Timestamp { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="AlertResponseEventArgs"/> class.
    /// </summary>
    public AlertResponseEventArgs(int? alertId, AcknowledgmentType responseType)
    {
        AlertId = alertId;
        ResponseType = responseType;
        Timestamp = DateTime.Now;
    }
}
