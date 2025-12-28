using HereForYou.Models;
using HereForYou.Services.Interfaces;

namespace HereForYou.Services;

/// <summary>
/// Service for analytics and insights.
/// </summary>
public class AnalyticsService : IAnalyticsService
{
    private readonly IDatabaseService _database;

    /// <summary>
    /// Initializes a new instance of the <see cref="AnalyticsService"/> class.
    /// </summary>
    public AnalyticsService(IDatabaseService database)
    {
        _database = database;
    }

    /// <inheritdoc/>
    public async Task<DailySummary> GetTodaySummaryAsync()
    {
        await _database.RecalculateDailySummaryAsync(DateTime.Today).ConfigureAwait(false);
        return await _database.GetOrCreateDailySummaryAsync(DateTime.Today).ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public async Task<List<DailySummary>> GetRecentSummariesAsync(int days)
    {
        var from = DateTime.Today.AddDays(-(days - 1));
        var to = DateTime.Today;

        // Ensure we have summaries for all days
        for (var date = from; date <= to; date = date.AddDays(1))
        {
            await _database.RecalculateDailySummaryAsync(date).ConfigureAwait(false);
        }

        return await _database.GetDailySummariesAsync(from, to).ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public async Task<float> CalculatePresenceScoreAsync(DateTime date)
    {
        await _database.RecalculateDailySummaryAsync(date).ConfigureAwait(false);
        var summary = await _database.GetOrCreateDailySummaryAsync(date).ConfigureAwait(false);
        return summary.PresenceScore ?? 100f;
    }

    /// <inheritdoc/>
    public async Task<WeeklyStats> GetWeeklyStatsAsync()
    {
        var today = DateTime.Today;
        var weekStart = today.AddDays(-(int)today.DayOfWeek + 1); // Monday
        if (today.DayOfWeek == DayOfWeek.Sunday)
        {
            weekStart = today.AddDays(-6);
        }

        var summaries = await GetRecentSummariesAsync(7).ConfigureAwait(false);

        var stats = new WeeklyStats
        {
            WeekStart = weekStart,
            DailyBreakdown = summaries
        };

        if (summaries.Count > 0)
        {
            stats.TotalScreenTime = TimeSpan.FromSeconds(summaries.Sum(s => s.TotalScreenTimeSeconds));
            stats.AverageDailyScreenTime = TimeSpan.FromSeconds(summaries.Average(s => s.TotalScreenTimeSeconds));
            stats.TotalDetections = summaries.Sum(s => s.TotalDetections);
            stats.TotalAlerts = summaries.Sum(s => s.TotalAlerts);
            stats.AveragePresenceScore = summaries.Where(s => s.PresenceScore.HasValue).Average(s => s.PresenceScore!.Value);

            int totalDetections = summaries.Sum(s => s.TotalDetections);
            int respondedDetections = summaries.Sum(s => s.RespondedDetections);
            stats.AverageResponseRate = totalDetections > 0
                ? (double)respondedDetections / totalDetections * 100
                : 0;
        }

        return stats;
    }

    /// <inheritdoc/>
    public async Task<MonthlyStats> GetMonthlyStatsAsync(int year, int month)
    {
        var firstDay = new DateTime(year, month, 1);
        var lastDay = firstDay.AddMonths(1).AddDays(-1);
        var daysInMonth = (lastDay - firstDay).Days + 1;

        // Calculate summaries for the month
        for (var date = firstDay; date <= lastDay && date <= DateTime.Today; date = date.AddDays(1))
        {
            await _database.RecalculateDailySummaryAsync(date).ConfigureAwait(false);
        }

        var summaries = await _database.GetDailySummariesAsync(firstDay, lastDay).ConfigureAwait(false);

        var stats = new MonthlyStats
        {
            Year = year,
            Month = month
        };

        if (summaries.Count > 0)
        {
            stats.TotalScreenTime = TimeSpan.FromSeconds(summaries.Sum(s => s.TotalScreenTimeSeconds));
            stats.AverageDailyScreenTime = TimeSpan.FromSeconds(summaries.Average(s => s.TotalScreenTimeSeconds));
            stats.TotalDetections = summaries.Sum(s => s.TotalDetections);
            stats.TotalAlerts = summaries.Sum(s => s.TotalAlerts);

            var scoredSummaries = summaries.Where(s => s.PresenceScore.HasValue).ToList();
            if (scoredSummaries.Count > 0)
            {
                stats.AveragePresenceScore = scoredSummaries.Average(s => s.PresenceScore!.Value);
                stats.BestDay = scoredSummaries.OrderByDescending(s => s.PresenceScore).FirstOrDefault();
                stats.MostChallengingDay = scoredSummaries.OrderBy(s => s.PresenceScore).FirstOrDefault();
            }
        }

        return stats;
    }

    /// <inheritdoc/>
    public async Task LogEventAsync(string eventType, string? data = null)
    {
        await _database.LogAnalyticsEventAsync(eventType, data).ConfigureAwait(false);
    }
}
