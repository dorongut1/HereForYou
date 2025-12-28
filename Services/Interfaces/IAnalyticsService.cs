using HereForYou.Models;

namespace HereForYou.Services.Interfaces;

/// <summary>
/// Interface for analytics and insights.
/// </summary>
public interface IAnalyticsService
{
    /// <summary>
    /// Gets today's summary.
    /// </summary>
    Task<DailySummary> GetTodaySummaryAsync();

    /// <summary>
    /// Gets summaries for the last N days.
    /// </summary>
    Task<List<DailySummary>> GetRecentSummariesAsync(int days);

    /// <summary>
    /// Calculates the presence score for a date.
    /// </summary>
    Task<float> CalculatePresenceScoreAsync(DateTime date);

    /// <summary>
    /// Gets weekly statistics.
    /// </summary>
    Task<WeeklyStats> GetWeeklyStatsAsync();

    /// <summary>
    /// Gets monthly statistics.
    /// </summary>
    Task<MonthlyStats> GetMonthlyStatsAsync(int year, int month);

    /// <summary>
    /// Logs an event.
    /// </summary>
    Task LogEventAsync(string eventType, string? data = null);
}

/// <summary>
/// Weekly statistics.
/// </summary>
public class WeeklyStats
{
    /// <summary>
    /// Gets or sets the start date of the week.
    /// </summary>
    public DateTime WeekStart { get; set; }

    /// <summary>
    /// Gets or sets the total screen time.
    /// </summary>
    public TimeSpan TotalScreenTime { get; set; }

    /// <summary>
    /// Gets or sets the average daily screen time.
    /// </summary>
    public TimeSpan AverageDailyScreenTime { get; set; }

    /// <summary>
    /// Gets or sets the total detections.
    /// </summary>
    public int TotalDetections { get; set; }

    /// <summary>
    /// Gets or sets the total alerts.
    /// </summary>
    public int TotalAlerts { get; set; }

    /// <summary>
    /// Gets or sets the average response rate.
    /// </summary>
    public double AverageResponseRate { get; set; }

    /// <summary>
    /// Gets or sets the average presence score.
    /// </summary>
    public float AveragePresenceScore { get; set; }

    /// <summary>
    /// Gets or sets daily breakdown.
    /// </summary>
    public List<DailySummary> DailyBreakdown { get; set; } = new();
}

/// <summary>
/// Monthly statistics.
/// </summary>
public class MonthlyStats
{
    /// <summary>
    /// Gets or sets the year.
    /// </summary>
    public int Year { get; set; }

    /// <summary>
    /// Gets or sets the month.
    /// </summary>
    public int Month { get; set; }

    /// <summary>
    /// Gets or sets the total screen time.
    /// </summary>
    public TimeSpan TotalScreenTime { get; set; }

    /// <summary>
    /// Gets or sets the average daily screen time.
    /// </summary>
    public TimeSpan AverageDailyScreenTime { get; set; }

    /// <summary>
    /// Gets or sets the total detections.
    /// </summary>
    public int TotalDetections { get; set; }

    /// <summary>
    /// Gets or sets the total alerts.
    /// </summary>
    public int TotalAlerts { get; set; }

    /// <summary>
    /// Gets or sets the average presence score.
    /// </summary>
    public float AveragePresenceScore { get; set; }

    /// <summary>
    /// Gets or sets the best day (highest presence score).
    /// </summary>
    public DailySummary? BestDay { get; set; }

    /// <summary>
    /// Gets or sets the most challenging day (lowest presence score).
    /// </summary>
    public DailySummary? MostChallengingDay { get; set; }
}
