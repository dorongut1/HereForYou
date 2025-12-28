using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HereForYou.Services.Interfaces;

namespace HereForYou.ViewModels;

public partial class MainViewModel : ObservableObject
{
    private readonly IDatabaseService _database;
    private readonly ISettingsService _settings;
    private readonly IAnalyticsService _analytics;

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
        IAnalyticsService analytics)
    {
        _database = database;
        _settings = settings;
        _analytics = analytics;
    }

    public async Task InitializeAsync()
    {
        await LoadTodayStatsAsync();
    }

    [RelayCommand]
    private async Task ToggleMonitoringAsync()
    {
        IsMonitoring = !IsMonitoring;

        if (IsMonitoring)
        {
            StatusMessage = "מאזין...";
            await _analytics.LogEventAsync("monitoring_started");
        }
        else
        {
            StatusMessage = "נעצר";
            await _analytics.LogEventAsync("monitoring_stopped");
        }

        await _settings.SetMonitoringEnabledAsync(IsMonitoring);
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
