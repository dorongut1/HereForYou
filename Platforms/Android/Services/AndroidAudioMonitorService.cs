#if ANDROID
using Android.Content;
using Android.Media;

using HereForYou.Models;
using HereForYou.Services.Interfaces;

namespace HereForYou.Platforms.Android.Services;

/// <summary>
/// Android implementation of audio monitoring service.
/// </summary>
public class AndroidAudioMonitorService : IAudioMonitorService, IDisposable
{
    private readonly IDatabaseService _database;
    private bool _isMonitoring;
    private List<KeywordProfile> _keywordProfiles = new();
    private AudioRecord? _audioRecord;
    private Thread? _monitoringThread;
    private CancellationTokenSource? _cancellationTokenSource;
    private float _currentAudioLevel;
    private bool _disposed;

    // Picovoice will be integrated here
    // For now, we implement a basic structure

    /// <inheritdoc/>
    public bool IsMonitoring => _isMonitoring;

    /// <inheritdoc/>
    public IReadOnlyList<string> ActiveKeywords => _keywordProfiles
        .Where(p => p.IsEnabled)
        .Select(p => p.DisplayName)
        .ToList();

    /// <inheritdoc/>
    public event EventHandler<KeywordDetectedEventArgs>? KeywordDetected;

    /// <inheritdoc/>
    public event EventHandler<AudioMonitorErrorEventArgs>? ErrorOccurred;

    /// <inheritdoc/>
    public event EventHandler<bool>? MonitoringStateChanged;

    /// <summary>
    /// Initializes a new instance of the <see cref="AndroidAudioMonitorService"/> class.
    /// </summary>
    public AndroidAudioMonitorService(IDatabaseService database)
    {
        _database = database;
    }

    /// <inheritdoc/>
    public async Task<bool> StartMonitoringAsync()
    {
        if (_isMonitoring)
        {
            return true;
        }

        try
        {
            // Check permission
            if (!await HasMicrophonePermissionAsync().ConfigureAwait(false))
            {
                var granted = await RequestMicrophonePermissionAsync().ConfigureAwait(false);
                if (!granted)
                {
                    OnError("Microphone permission not granted", null, false);
                    return false;
                }
            }

            // Load keyword profiles
            _keywordProfiles = await _database.GetEnabledKeywordProfilesAsync().ConfigureAwait(false);

            // Initialize audio recording
            int sampleRate = 16000;
            var channelConfig = ChannelIn.Mono;
            var encoding = Encoding.Pcm16bit;
            int bufferSize = AudioRecord.GetMinBufferSize(sampleRate, channelConfig, encoding) * 2;

            _audioRecord = new AudioRecord(
                AudioSource.Mic,
                sampleRate,
                channelConfig,
                encoding,
                bufferSize);

            if (_audioRecord.State != State.Initialized)
            {
                OnError("Failed to initialize audio recorder", null, true);
                return false;
            }

            // Start recording
            _audioRecord.StartRecording();
            _cancellationTokenSource = new CancellationTokenSource();

            // Start monitoring thread
            _monitoringThread = new Thread(() => MonitoringLoop(_cancellationTokenSource.Token));
            _monitoringThread.Start();

            _isMonitoring = true;
            MonitoringStateChanged?.Invoke(this, true);

            await _database.LogAnalyticsEventAsync(EventTypes.MonitoringStarted).ConfigureAwait(false);

            return true;
        }
        catch (Exception ex)
        {
            OnError($"Failed to start monitoring: {ex.Message}", ex, true);
            return false;
        }
    }

    private void MonitoringLoop(CancellationToken cancellationToken)
    {
        byte[] buffer = new byte[1024];

        while (!cancellationToken.IsCancellationRequested && _audioRecord != null)
        {
            try
            {
                int bytesRead = _audioRecord.Read(buffer, 0, buffer.Length);

                if (bytesRead > 0)
                {
                    // Calculate audio level
                    _currentAudioLevel = CalculateAudioLevel(buffer, bytesRead);

                    // TODO: Send to Picovoice for keyword detection
                    // For now, this is a placeholder
                    // When Picovoice is integrated, it will process the audio
                    // and call OnKeywordDetected when a keyword is recognized
                }
            }
            catch (Exception ex)
            {
                if (!cancellationToken.IsCancellationRequested)
                {
                    MainThread.BeginInvokeOnMainThread(() =>
                    {
                        OnError($"Error in monitoring loop: {ex.Message}", ex, true);
                    });
                }
            }
        }
    }

    private float CalculateAudioLevel(byte[] buffer, int length)
    {
        // Calculate RMS of audio samples
        long sum = 0;
        for (int i = 0; i < length - 1; i += 2)
        {
            short sample = (short)(buffer[i] | (buffer[i + 1] << 8));
            sum += sample * sample;
        }

        double rms = Math.Sqrt(sum / (length / 2.0));
        return (float)(rms / 32768.0); // Normalize to 0-1
    }

    /// <inheritdoc/>
    public async Task StopMonitoringAsync()
    {
        if (!_isMonitoring)
        {
            return;
        }

        try
        {
            _cancellationTokenSource?.Cancel();
            _monitoringThread?.Join(1000);

            _audioRecord?.Stop();
            _audioRecord?.Release();
            _audioRecord = null;

            _isMonitoring = false;
            MonitoringStateChanged?.Invoke(this, false);

            await _database.LogAnalyticsEventAsync(EventTypes.MonitoringStopped).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            OnError($"Error stopping monitoring: {ex.Message}", ex, true);
        }
    }

    /// <inheritdoc/>
    public async Task<bool> HasMicrophonePermissionAsync()
    {
        var status = await Permissions.CheckStatusAsync<Permissions.Microphone>().ConfigureAwait(false);
        return status == PermissionStatus.Granted;
    }

    /// <inheritdoc/>
    public async Task<bool> RequestMicrophonePermissionAsync()
    {
        var status = await Permissions.RequestAsync<Permissions.Microphone>().ConfigureAwait(false);
        return status == PermissionStatus.Granted;
    }

    /// <inheritdoc/>
    public async Task UpdateKeywordsAsync(IEnumerable<KeywordProfile> profiles)
    {
        _keywordProfiles = profiles.ToList();

        // If monitoring, restart with new keywords
        if (_isMonitoring)
        {
            await StopMonitoringAsync().ConfigureAwait(false);
            await StartMonitoringAsync().ConfigureAwait(false);
        }
    }

    /// <inheritdoc/>
    public float GetCurrentAudioLevel() => _currentAudioLevel;

    /// <summary>
    /// Called when a keyword is detected (will be called by Picovoice).
    /// </summary>
    protected void OnKeywordDetected(string keyword, float confidence)
    {
        KeywordDetected?.Invoke(this, new KeywordDetectedEventArgs(keyword, confidence));
    }

    private void OnError(string message, Exception? exception, bool isRecoverable)
    {
        ErrorOccurred?.Invoke(this, new AudioMonitorErrorEventArgs(message, exception, isRecoverable));
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Releases resources.
    /// </summary>
    protected virtual void Dispose(bool disposing)
    {
        if (_disposed)
        {
            return;
        }

        if (disposing)
        {
            _cancellationTokenSource?.Cancel();
            _cancellationTokenSource?.Dispose();
            _audioRecord?.Release();
            _audioRecord?.Dispose();
        }

        _disposed = true;
    }
}
#endif
