using HereForYou.Models;
using HereForYou.Services.Interfaces;

namespace HereForYou.Services;

/// <summary>
/// Notification service for showing alerts to the user.
/// </summary>
public class NotificationService : INotificationService
{
    public event EventHandler<AlertResponseEventArgs>? AlertResponded;

    public async Task ShowAlertAsync(string title, string message, AlertLevelType level, int? alertId = null)
    {
        System.Diagnostics.Debug.WriteLine($"[Notification] [{level}] {title}: {message}");
        await Task.CompletedTask;
    }

    public async Task ShowOverlayAlertAsync(string title, string message, int? alertId = null)
    {
        System.Diagnostics.Debug.WriteLine($"[Notification] [OVERLAY] {title}: {message}");
        await Task.CompletedTask;
    }

    public async Task DismissOverlayAsync()
    {
        System.Diagnostics.Debug.WriteLine("[Notification] Dismissing overlay");
        await Task.CompletedTask;
    }

    public async Task ShowNotificationAsync(string title, string message)
    {
        System.Diagnostics.Debug.WriteLine($"[Notification] {title}: {message}");
        await Task.CompletedTask;
    }

    public async Task ShowMonitoringNotificationAsync()
    {
        System.Diagnostics.Debug.WriteLine("[Notification] Showing monitoring notification");
        await Task.CompletedTask;
    }

    public async Task HideMonitoringNotificationAsync()
    {
        System.Diagnostics.Debug.WriteLine("[Notification] Hiding monitoring notification");
        await Task.CompletedTask;
    }

    public Task<bool> HasNotificationPermissionAsync()
    {
        return Task.FromResult(true);
    }

    public Task<bool> RequestNotificationPermissionAsync()
    {
        return Task.FromResult(true);
    }

    public Task<bool> HasOverlayPermissionAsync()
    {
        return Task.FromResult(true);
    }

    public Task<bool> RequestOverlayPermissionAsync()
    {
        return Task.FromResult(true);
    }

    public async Task PlayAlertSoundAsync(AlertLevelType level)
    {
        System.Diagnostics.Debug.WriteLine($"[Notification] Playing {level} sound");
        await Task.CompletedTask;
    }

    public async Task VibrateAsync(AlertLevelType level)
    {
        System.Diagnostics.Debug.WriteLine($"[Notification] Vibrating for {level}");
        await Task.CompletedTask;
    }
}
