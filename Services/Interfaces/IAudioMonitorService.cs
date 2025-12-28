using HereForYou.Models;

namespace HereForYou.Services.Interfaces;

/// <summary>
/// Interface for audio monitoring and keyword detection.
/// </summary>
public interface IAudioMonitorService
{
    /// <summary>
    /// Gets a value indicating whether monitoring is currently active.
    /// </summary>
    bool IsMonitoring { get; }

    /// <summary>
    /// Gets the list of keywords being monitored.
    /// </summary>
    IReadOnlyList<string> ActiveKeywords { get; }

    /// <summary>
    /// Event raised when a keyword is detected.
    /// </summary>
    event EventHandler<KeywordDetectedEventArgs>? KeywordDetected;

    /// <summary>
    /// Event raised when an error occurs.
    /// </summary>
    event EventHandler<AudioMonitorErrorEventArgs>? ErrorOccurred;

    /// <summary>
    /// Event raised when monitoring state changes.
    /// </summary>
    event EventHandler<bool>? MonitoringStateChanged;

    /// <summary>
    /// Starts monitoring for keywords.
    /// </summary>
    /// <returns>True if monitoring started successfully.</returns>
    Task<bool> StartMonitoringAsync();

    /// <summary>
    /// Stops monitoring.
    /// </summary>
    Task StopMonitoringAsync();

    /// <summary>
    /// Checks if the app has microphone permission.
    /// </summary>
    Task<bool> HasMicrophonePermissionAsync();

    /// <summary>
    /// Requests microphone permission.
    /// </summary>
    Task<bool> RequestMicrophonePermissionAsync();

    /// <summary>
    /// Updates the keywords to monitor.
    /// </summary>
    Task UpdateKeywordsAsync(IEnumerable<KeywordProfile> profiles);

    /// <summary>
    /// Gets the current audio level (0.0 - 1.0).
    /// </summary>
    float GetCurrentAudioLevel();
}

/// <summary>
/// Event arguments for keyword detection.
/// </summary>
public class KeywordDetectedEventArgs : EventArgs
{
    /// <summary>
    /// Gets the detected keyword.
    /// </summary>
    public string Keyword { get; }

    /// <summary>
    /// Gets the detection confidence.
    /// </summary>
    public float Confidence { get; }

    /// <summary>
    /// Gets the detection timestamp.
    /// </summary>
    public DateTime Timestamp { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="KeywordDetectedEventArgs"/> class.
    /// </summary>
    public KeywordDetectedEventArgs(string keyword, float confidence)
    {
        Keyword = keyword;
        Confidence = confidence;
        Timestamp = DateTime.Now;
    }
}

/// <summary>
/// Event arguments for audio monitor errors.
/// </summary>
public class AudioMonitorErrorEventArgs : EventArgs
{
    /// <summary>
    /// Gets the error message.
    /// </summary>
    public string Message { get; }

    /// <summary>
    /// Gets the exception if available.
    /// </summary>
    public Exception? Exception { get; }

    /// <summary>
    /// Gets a value indicating whether the error is recoverable.
    /// </summary>
    public bool IsRecoverable { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="AudioMonitorErrorEventArgs"/> class.
    /// </summary>
    public AudioMonitorErrorEventArgs(string message, Exception? exception = null, bool isRecoverable = true)
    {
        Message = message;
        Exception = exception;
        IsRecoverable = isRecoverable;
    }
}
