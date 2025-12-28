using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HereForYou.Models;
using HereForYou.Services.Interfaces;

namespace HereForYou.ViewModels;

public partial class InsightsViewModel : ObservableObject
{
    private readonly IDatabaseService _database;

    [ObservableProperty]
    private string _todayScreenTime = "00:00";

    [ObservableProperty]
    private int _todayDetections;

    [ObservableProperty]
    private int _todayAlerts;

    [ObservableProperty]
    private string _weekScreenTime = "00:00";

    [ObservableProperty]
    private int _weekDetections;

    [ObservableProperty]
    private float _responseRate;

    [ObservableProperty]
    private List<DailySummary> _recentSummaries = new();

    [ObservableProperty]
    private List<DetectionEvent> _recentDetections = new();

    public InsightsViewModel(IDatabaseService database)
    {
        _database = database;
    }

    public async Task InitializeAsync()
    {
        await LoadStatsAsync();
    }

    [RelayCommand]
    private async Task RefreshAsync()
    {
        await LoadStatsAsync();
    }

    private async Task LoadStatsAsync()
    {
        var today = DateTime.Today;
        var weekAgo = today.AddDays(-7);

        // Today stats
        var todayScreenTimeSpan = await _database.GetTotalScreenTimeAsync(today);
        TodayScreenTime = $"{(int)todayScreenTimeSpan.TotalHours:D2}:{todayScreenTimeSpan.Minutes:D2}";

        var todayDetectionsList = await _database.GetDetectionEventsAsync(today, DateTime.Now);
        TodayDetections = todayDetectionsList.Count;

        var todayAlertsList = await _database.GetAlertsAsync(today, DateTime.Now);
        TodayAlerts = todayAlertsList.Count;

        // Week stats
        var weekSummaries = await _database.GetDailySummariesAsync(weekAgo, today);
        var totalWeekSeconds = weekSummaries.Sum(s => s.TotalScreenTimeSeconds);
        var weekScreenTimeSpan = TimeSpan.FromSeconds(totalWeekSeconds);
        WeekScreenTime = $"{(int)weekScreenTimeSpan.TotalHours:D2}:{weekScreenTimeSpan.Minutes:D2}";

        WeekDetections = weekSummaries.Sum(s => s.TotalDetections);

        // Response rate
        var totalDetections = weekSummaries.Sum(s => s.TotalDetections);
        var totalResponded = weekSummaries.Sum(s => s.RespondedDetections);
        ResponseRate = totalDetections > 0 ? (float)totalResponded / totalDetections * 100 : 0;

        // Recent summaries (last 7 days)
        RecentSummaries = weekSummaries.OrderByDescending(s => s.Date).ToList();

        // Recent detections (last 20)
        RecentDetections = await _database.GetRecentDetectionEventsAsync(20);
    }
}
