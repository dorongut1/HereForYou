using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HereForYou.Models;
using HereForYou.Services.Interfaces;

namespace HereForYou.ViewModels;

public partial class MainViewModel : ObservableObject
{
    private readonly IDatabaseService _database;
    private readonly ISettingsService _settings;
    private readonly IAnalyticsService _analytics;
    private readonly IAudioMonitorService _audioMonitor;
    private readonly IAlertCoordinatorService _alertCoordinator;

    [ObservableProperty]
    private bool _isMonitoring;

    [ObservableProperty]
    private string _statusMessage = "לא פעיל";

    [ObservableProperty]
    private int _detectionCount;

    [ObservableProperty]
    private string _todayScreenTime = "00:00";

    [ObservableProperty]
    private int _todayDetections;

    public MainViewModel(
        IDatabaseService database,
        ISettingsService settings,
        IAnalyticsService analytics,
        IAudioMonitorService audioMonitor,
        IAlertCoordinatorService alertCoordinator)
    {
        _database = database;
        _settings = settings;
        _analytics = analytics;
        _audioMonitor = audioMonitor;
        _alertCoordinator = alertCoordinator;

        // Subscribe to events
        _audioMonitor.KeywordDetected += OnKeywordDetected;
        _audioMonitor.MonitoringStateChanged += OnMonitoringStateChanged;
        _alertCoordinator.AlertTriggered += OnAlertTriggered;
    }

    public async Task InitializeAsync()
    {
        await _settings.InitializeAsync();
        await _alertCoordinator.InitializeAsync();
        await LoadTodayStatsAsync();

        // Restore monitoring state
        if (_settings.IsMonitoringEnabled && !_audioMonitor.IsMonitoring)
        {
            await StartMonitoringAsync();
        }
    }

    [RelayCommand]
    private async Task ToggleMonitoringAsync()
    {
        if (IsMonitoring)
        {
            await StopMonitoringAsync();
        }
        else
        {
            await StartMonitoringAsync();
        }
    }

    private async Task StartMonitoringAsync()
    {
        var success = await _audioMonitor.StartMonitoringAsync();
        if (success)
        {
            IsMonitoring = true;
            StatusMessage = "מאזין...";
            await _analytics.LogEventAsync("monitoring_started");
            await _settings.SetMonitoringEnabledAsync(true);
        }
    }

    private async Task StopMonitoringAsync()
    {
        await _audioMonitor.StopMonitoringAsync();
        IsMonitoring = false;
        StatusMessage = "נעצר";
        await _analytics.LogEventAsync("monitoring_stopped");
        await _settings.SetMonitoringEnabledAsync(false);
    }

    private async void OnKeywordDetected(object? sender, KeywordDetectedEventArgs e)
    {
        // Create detection event
        var detection = new DetectionEvent
        {
            Keyword = e.Keyword,
            Confidence = e.Confidence,
            Timestamp = DateTime.Now,
            WasPartOfAlert = false,
            WasRespondedTo = false
        };

        // Process through alert coordinator
        await _alertCoordinator.ProcessDetectionAsync(detection);

        // Update detection count
        DetectionCount = _alertCoordinator.PendingDetectionCount;

        // Refresh today's stats
        await LoadTodayStatsAsync();
    }

    private void OnMonitoringStateChanged(object? sender, bool isMonitoring)
    {
        IsMonitoring = isMonitoring;
        StatusMessage = isMonitoring ? "מאזין..." : "נעצר";
    }

    private async void OnAlertTriggered(object? sender, AlertTriggeredEventArgs e)
    {
        // Clear detection count when alert is triggered
        DetectionCount = 0;

        // Refresh stats
        await LoadTodayStatsAsync();
    }

    private async Task LoadTodayStatsAsync()
    {
        var today = DateTime.Today;

        // Load screen time
        var screenTime = await _database.GetTotalScreenTimeAsync(today);
        TodayScreenTime = $"{(int)screenTime.TotalHours:D2}:{screenTime.Minutes:D2}";

        // Load detections
        var detections = await _database.GetDetectionEventsAsync(today, DateTime.Now);
        TodayDetections = detections.Count;
    }
}
