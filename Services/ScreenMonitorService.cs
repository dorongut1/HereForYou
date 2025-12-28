using HereForYou.Models;
using HereForYou.Services.Interfaces;

namespace HereForYou.Services;

/// <summary>
/// Monitors app screen time using MAUI lifecycle events.
/// </summary>
public class ScreenMonitorService : IScreenMonitorService
{
    private readonly IDatabaseService _database;
    private ScreenTimeSession? _currentSession;
    private System.Timers.Timer? _updateTimer;

    public bool IsMonitoring { get; private set; }
    public ScreenTimeSession? CurrentSession => _currentSession;

    public event EventHandler<ScreenTimeSession>? SessionStarted;
    public event EventHandler<ScreenTimeSession>? SessionEnded;
    public event EventHandler<ScreenTimeSession>? SessionUpdated;

    public ScreenMonitorService(IDatabaseService database)
    {
        _database = database;
    }

    public async Task StartMonitoringAsync()
    {
        if (IsMonitoring) return;

        IsMonitoring = true;

        // Start new session
        _currentSession = new ScreenTimeSession
        {
            StartTime = DateTime.Now
            // EndTime is null (active session)
            // IsActive is computed property (EndTime == null)
        };

        // Save to database
        await _database.SaveScreenTimeSessionAsync(_currentSession);
        SessionStarted?.Invoke(this, _currentSession);

        // Update session duration every minute
        _updateTimer = new System.Timers.Timer(60000); // 1 minute
        _updateTimer.Elapsed += async (s, e) => await UpdateCurrentSessionAsync();
        _updateTimer.Start();

        System.Diagnostics.Debug.WriteLine("[ScreenMonitor] Monitoring started");
    }

    public async Task StopMonitoringAsync()
    {
        if (!IsMonitoring || _currentSession == null) return;

        IsMonitoring = false;
        _updateTimer?.Stop();
        _updateTimer?.Dispose();
        _updateTimer = null;

        // End current session
        _currentSession.EndTime = DateTime.Now;
        _currentSession.DurationSeconds = (int)(DateTime.Now - _currentSession.StartTime).TotalSeconds;

        await _database.UpdateScreenTimeSessionAsync(_currentSession);
        SessionEnded?.Invoke(this, _currentSession);

        System.Diagnostics.Debug.WriteLine($"[ScreenMonitor] Session ended: {_currentSession.DurationSeconds}s");

        _currentSession = null;
    }

    public async Task MarkSessionInterruptedAsync()
    {
        if (_currentSession == null) return;

        _currentSession.WasInterrupted = true;
        _currentSession.InterruptionCount++;
        await _database.UpdateScreenTimeSessionAsync(_currentSession);

        System.Diagnostics.Debug.WriteLine("[ScreenMonitor] Session marked as interrupted");
    }

    public async Task<TimeSpan> GetTodayScreenTimeAsync()
    {
        return await _database.GetTotalScreenTimeAsync(DateTime.Today);
    }

    public async Task<TimeSpan> GetScreenTimeAsync(DateTime date)
    {
        return await _database.GetTotalScreenTimeAsync(date);
    }

    public Task<bool> HasUsageStatsPermissionAsync()
    {
        // Not needed on Windows/desktop
        return Task.FromResult(true);
    }

    public Task RequestUsageStatsPermissionAsync()
    {
        // Not needed on Windows/desktop
        return Task.CompletedTask;
    }

    private async Task UpdateCurrentSessionAsync()
    {
        if (_currentSession == null || !IsMonitoring) return;

        _currentSession.DurationSeconds = (int)(DateTime.Now - _currentSession.StartTime).TotalSeconds;
        await _database.UpdateScreenTimeSessionAsync(_currentSession);
        SessionUpdated?.Invoke(this, _currentSession);
    }
}
