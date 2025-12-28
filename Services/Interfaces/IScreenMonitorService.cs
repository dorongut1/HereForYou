using HereForYou.Models;

namespace HereForYou.Services.Interfaces;

/// <summary>
/// Interface for screen time monitoring.
/// </summary>
public interface IScreenMonitorService
{
    /// <summary>
    /// Gets a value indicating whether monitoring is active.
    /// </summary>
    bool IsMonitoring { get; }

    /// <summary>
    /// Gets the current session if active.
    /// </summary>
    ScreenTimeSession? CurrentSession { get; }

    /// <summary>
    /// Event raised when a session starts.
    /// </summary>
    event EventHandler<ScreenTimeSession>? SessionStarted;

    /// <summary>
    /// Event raised when a session ends.
    /// </summary>
    event EventHandler<ScreenTimeSession>? SessionEnded;

    /// <summary>
    /// Event raised when session is updated.
    /// </summary>
    event EventHandler<ScreenTimeSession>? SessionUpdated;

    /// <summary>
    /// Starts monitoring screen time.
    /// </summary>
    Task StartMonitoringAsync();

    /// <summary>
    /// Stops monitoring screen time.
    /// </summary>
    Task StopMonitoringAsync();

    /// <summary>
    /// Marks current session as interrupted.
    /// </summary>
    Task MarkSessionInterruptedAsync();

    /// <summary>
    /// Gets today's total screen time.
    /// </summary>
    Task<TimeSpan> GetTodayScreenTimeAsync();

    /// <summary>
    /// Gets screen time for a specific date.
    /// </summary>
    Task<TimeSpan> GetScreenTimeAsync(DateTime date);

    /// <summary>
    /// Checks if usage stats permission is granted (Android).
    /// </summary>
    Task<bool> HasUsageStatsPermissionAsync();

    /// <summary>
    /// Requests usage stats permission (Android).
    /// </summary>
    Task RequestUsageStatsPermissionAsync();
}
