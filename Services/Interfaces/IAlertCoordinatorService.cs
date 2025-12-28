using HereForYou.Models;

namespace HereForYou.Services.Interfaces;

/// <summary>
/// Interface for coordinating alerts based on detection events.
/// </summary>
public interface IAlertCoordinatorService
{
    /// <summary>
    /// Gets the current pending detection count.
    /// </summary>
    int PendingDetectionCount { get; }

    /// <summary>
    /// Event raised when an alert should be shown.
    /// </summary>
    event EventHandler<AlertTriggeredEventArgs>? AlertTriggered;

    /// <summary>
    /// Initializes the coordinator.
    /// </summary>
    Task InitializeAsync();

    /// <summary>
    /// Processes a new detection event.
    /// </summary>
    Task ProcessDetectionAsync(DetectionEvent detection);

    /// <summary>
    /// Handles an alert response.
    /// </summary>
    Task HandleAlertResponseAsync(int alertId, AcknowledgmentType response);

    /// <summary>
    /// Clears pending detections.
    /// </summary>
    void ClearPendingDetections();
}

/// <summary>
/// Event arguments when an alert is triggered.
/// </summary>
public class AlertTriggeredEventArgs : EventArgs
{
    /// <summary>
    /// Gets the alert.
    /// </summary>
    public Alert Alert { get; }

    /// <summary>
    /// Gets the detection events that triggered this alert.
    /// </summary>
    public IReadOnlyList<DetectionEvent> Detections { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="AlertTriggeredEventArgs"/> class.
    /// </summary>
    public AlertTriggeredEventArgs(Alert alert, IReadOnlyList<DetectionEvent> detections)
    {
        Alert = alert;
        Detections = detections;
    }
}
