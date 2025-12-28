using HereForYou.Models;
using HereForYou.Services.Interfaces;

namespace HereForYou.Services;

/// <summary>
/// Mock audio monitor service for testing without Picovoice.
/// </summary>
public class MockAudioMonitorService : IAudioMonitorService
{
    private readonly IDatabaseService _database;
    private readonly IAlertCoordinatorService _alertCoordinator;
    private bool _isMonitoring;
    private CancellationTokenSource? _cancellationTokenSource;
    private List<string> _activeKeywords = new() { "אמא", "אבא" };

    public event EventHandler<KeywordDetectedEventArgs>? KeywordDetected;
    public event EventHandler<AudioMonitorErrorEventArgs>? ErrorOccurred;
    public event EventHandler<bool>? MonitoringStateChanged;

    public bool IsMonitoring => _isMonitoring;
    public IReadOnlyList<string> ActiveKeywords => _activeKeywords.AsReadOnly();

    public MockAudioMonitorService(
        IDatabaseService database,
        IAlertCoordinatorService alertCoordinator)
    {
        _database = database;
        _alertCoordinator = alertCoordinator;
    }

    public async Task<bool> StartMonitoringAsync()
    {
        if (_isMonitoring)
            return true;

        _isMonitoring = true;
        MonitoringStateChanged?.Invoke(this, true);

        // Start mock detection loop
        _cancellationTokenSource = new CancellationTokenSource();
        _ = Task.Run(() => MockDetectionLoop(_cancellationTokenSource.Token));

        return await Task.FromResult(true);
    }

    public Task StopMonitoringAsync()
    {
        if (!_isMonitoring)
            return Task.CompletedTask;

        _isMonitoring = false;
        _cancellationTokenSource?.Cancel();
        MonitoringStateChanged?.Invoke(this, false);

        return Task.CompletedTask;
    }

    public Task<bool> HasMicrophonePermissionAsync()
    {
        return Task.FromResult(true);
    }

    public Task<bool> RequestMicrophonePermissionAsync()
    {
        return Task.FromResult(true);
    }

    public Task UpdateKeywordsAsync(IEnumerable<KeywordProfile> profiles)
    {
        _activeKeywords = profiles.Select(p => p.DisplayName).ToList();
        return Task.CompletedTask;
    }

    public float GetCurrentAudioLevel()
    {
        return new Random().NextSingle() * 0.5f; // 0-0.5
    }

    private async Task MockDetectionLoop(CancellationToken cancellationToken)
    {
        var random = new Random();

        while (!cancellationToken.IsCancellationRequested)
        {
            // Wait 10-30 seconds between detections
            await Task.Delay(TimeSpan.FromSeconds(random.Next(10, 30)), cancellationToken);

            if (cancellationToken.IsCancellationRequested)
                break;

            // Random keyword
            var keyword = _activeKeywords[random.Next(_activeKeywords.Count)];
            var confidence = (float)(0.7 + random.NextDouble() * 0.3); // 0.7-1.0

            // Trigger detection
            KeywordDetected?.Invoke(this, new KeywordDetectedEventArgs(keyword, confidence));
        }
    }
}
