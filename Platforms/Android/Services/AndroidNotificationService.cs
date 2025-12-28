#if ANDROID
using Android.App;
using Android.Content;
using Android.OS;

using AndroidX.Core.App;

using HereForYou.Models;
using HereForYou.Services.Interfaces;

namespace HereForYou.Platforms.Android.Services;

/// <summary>
/// Android implementation of notification service.
/// </summary>
public class AndroidNotificationService : INotificationService
{
    private const string ChannelId = "hereforyou_alerts";
    private const string MonitoringChannelId = "hereforyou_monitoring";
    private const int MonitoringNotificationId = 1000;
    private const int AlertNotificationId = 2000;
    private int _alertCounter;

    private readonly Context _context;
    private NotificationManager? _notificationManager;

    /// <inheritdoc/>
    public event EventHandler<AlertResponseEventArgs>? AlertResponded;

    /// <summary>
    /// Initializes a new instance of the <see cref="AndroidNotificationService"/> class.
    /// </summary>
    public AndroidNotificationService()
    {
        _context = Platform.AppContext;
        CreateNotificationChannels();
    }

    private void CreateNotificationChannels()
    {
        if (Build.VERSION.SdkInt < BuildVersionCodes.O)
        {
            return;
        }

        _notificationManager = (NotificationManager?)_context.GetSystemService(Context.NotificationService);

        // Alert channel - high priority
        var alertChannel = new NotificationChannel(
            ChannelId,
            "Alerts",
            NotificationImportance.High)
        {
            Description = "Alerts when your child is calling"
        };
        alertChannel.EnableVibration(true);
        alertChannel.EnableLights(true);
        _notificationManager?.CreateNotificationChannel(alertChannel);

        // Monitoring channel - low priority
        var monitoringChannel = new NotificationChannel(
            MonitoringChannelId,
            "Monitoring",
            NotificationImportance.Low)
        {
            Description = "Shows when monitoring is active"
        };
        _notificationManager?.CreateNotificationChannel(monitoringChannel);
    }

    /// <inheritdoc/>
    public async Task ShowAlertAsync(string title, string message, AlertLevelType level, int? alertId = null)
    {
        await Task.Run(() =>
        {
            var builder = new NotificationCompat.Builder(_context, ChannelId)
                .SetContentTitle(title)
                .SetContentText(message)
                .SetSmallIcon(Resource.Drawable.notification_icon)
                .SetAutoCancel(true)
                .SetPriority(level == AlertLevelType.Critical
                    ? NotificationCompat.PriorityHigh
                    : NotificationCompat.PriorityDefault);

            // Add action buttons
            if (alertId.HasValue)
            {
                var handledIntent = CreateActionIntent("HANDLED", alertId.Value);
                var handledPending = PendingIntent.GetBroadcast(
                    _context,
                    alertId.Value * 10,
                    handledIntent,
                    PendingIntentFlags.Immutable | PendingIntentFlags.UpdateCurrent);

                var snoozeIntent = CreateActionIntent("SNOOZED", alertId.Value);
                var snoozePending = PendingIntent.GetBroadcast(
                    _context,
                    alertId.Value * 10 + 1,
                    snoozeIntent,
                    PendingIntentFlags.Immutable | PendingIntentFlags.UpdateCurrent);

                builder.AddAction(0, "טיפלתי", handledPending);
                builder.AddAction(0, "עוד רגע", snoozePending);
            }

            _notificationManager?.Notify(AlertNotificationId + _alertCounter++, builder.Build());
        }).ConfigureAwait(false);
    }

    private Intent CreateActionIntent(string action, int alertId)
    {
        var intent = new Intent(_context, typeof(AlertActionReceiver));
        intent.SetAction(action);
        intent.PutExtra("AlertId", alertId);
        return intent;
    }

    /// <inheritdoc/>
    public async Task ShowOverlayAlertAsync(string title, string message, int? alertId = null)
    {
        // Check overlay permission
        if (!await HasOverlayPermissionAsync().ConfigureAwait(false))
        {
            // Fall back to regular notification
            await ShowAlertAsync(title, message, AlertLevelType.Critical, alertId).ConfigureAwait(false);
            return;
        }

        // Show full-screen overlay
        await MainThread.InvokeOnMainThreadAsync(() =>
        {
            // Create and show overlay window
            // This would use WindowManager to display a full-screen view
            // For now, we show a high-priority notification
            var builder = new NotificationCompat.Builder(_context, ChannelId)
                .SetContentTitle(title)
                .SetContentText(message)
                .SetSmallIcon(Resource.Drawable.notification_icon)
                .SetPriority(NotificationCompat.PriorityMax)
                .SetCategory(NotificationCompat.CategoryCall)
                .SetFullScreenIntent(CreateFullScreenIntent(alertId), true)
                .SetAutoCancel(true);

            if (alertId.HasValue)
            {
                var handledIntent = CreateActionIntent("HANDLED", alertId.Value);
                var handledPending = PendingIntent.GetBroadcast(
                    _context,
                    alertId.Value * 10,
                    handledIntent,
                    PendingIntentFlags.Immutable | PendingIntentFlags.UpdateCurrent);

                var dismissIntent = CreateActionIntent("DISMISSED", alertId.Value);
                var dismissPending = PendingIntent.GetBroadcast(
                    _context,
                    alertId.Value * 10 + 2,
                    dismissIntent,
                    PendingIntentFlags.Immutable | PendingIntentFlags.UpdateCurrent);

                builder.AddAction(0, "אני מטפל בזה", handledPending);
                builder.AddAction(0, "דחה", dismissPending);
            }

            _notificationManager?.Notify(AlertNotificationId, builder.Build());
        }).ConfigureAwait(false);
    }

    private PendingIntent? CreateFullScreenIntent(int? alertId)
    {
        // Create intent to open app
        var intent = new Intent(_context, typeof(MainActivity));
        intent.SetFlags(ActivityFlags.NewTask | ActivityFlags.ClearTop);
        if (alertId.HasValue)
        {
            intent.PutExtra("AlertId", alertId.Value);
        }

        return PendingIntent.GetActivity(
            _context,
            0,
            intent,
            PendingIntentFlags.Immutable | PendingIntentFlags.UpdateCurrent);
    }

    /// <inheritdoc/>
    public Task DismissOverlayAsync()
    {
        _notificationManager?.Cancel(AlertNotificationId);
        return Task.CompletedTask;
    }

    /// <inheritdoc/>
    public async Task ShowNotificationAsync(string title, string message)
    {
        await Task.Run(() =>
        {
            var builder = new NotificationCompat.Builder(_context, ChannelId)
                .SetContentTitle(title)
                .SetContentText(message)
                .SetSmallIcon(Resource.Drawable.notification_icon)
                .SetAutoCancel(true)
                .SetPriority(NotificationCompat.PriorityDefault);

            _notificationManager?.Notify(AlertNotificationId + _alertCounter++, builder.Build());
        }).ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public Task ShowMonitoringNotificationAsync()
    {
        var builder = new NotificationCompat.Builder(_context, MonitoringChannelId)
            .SetContentTitle("HereForYou פעיל")
            .SetContentText("מאזין לפניות הילדים שלך")
            .SetSmallIcon(Resource.Drawable.notification_icon)
            .SetOngoing(true)
            .SetPriority(NotificationCompat.PriorityLow);

        _notificationManager?.Notify(MonitoringNotificationId, builder.Build());
        return Task.CompletedTask;
    }

    /// <inheritdoc/>
    public Task HideMonitoringNotificationAsync()
    {
        _notificationManager?.Cancel(MonitoringNotificationId);
        return Task.CompletedTask;
    }

    /// <inheritdoc/>
    public async Task<bool> HasNotificationPermissionAsync()
    {
        if (Build.VERSION.SdkInt < BuildVersionCodes.Tiramisu)
        {
            return true;
        }

        var status = await Permissions.CheckStatusAsync<Permissions.PostNotifications>().ConfigureAwait(false);
        return status == PermissionStatus.Granted;
    }

    /// <inheritdoc/>
    public async Task<bool> RequestNotificationPermissionAsync()
    {
        if (Build.VERSION.SdkInt < BuildVersionCodes.Tiramisu)
        {
            return true;
        }

        var status = await Permissions.RequestAsync<Permissions.PostNotifications>().ConfigureAwait(false);
        return status == PermissionStatus.Granted;
    }

    /// <inheritdoc/>
    public Task<bool> HasOverlayPermissionAsync()
    {
        if (Build.VERSION.SdkInt < BuildVersionCodes.M)
        {
            return Task.FromResult(true);
        }

        return Task.FromResult(global::Android.Provider.Settings.CanDrawOverlays(_context));
    }

    /// <inheritdoc/>
    public Task<bool> RequestOverlayPermissionAsync()
    {
        if (Build.VERSION.SdkInt < BuildVersionCodes.M)
        {
            return Task.FromResult(true);
        }

        var intent = new Intent(
            global::Android.Provider.Settings.ActionManageOverlayPermission,
            global::Android.Net.Uri.Parse($"package:{_context.PackageName}"));
        intent.SetFlags(ActivityFlags.NewTask);
        _context.StartActivity(intent);

        return Task.FromResult(false); // User needs to grant manually
    }

    /// <inheritdoc/>
    public async Task PlayAlertSoundAsync(AlertLevelType level)
    {
        await Task.Run(() =>
        {
            try
            {
                var uri = global::Android.Media.RingtoneManager.GetDefaultUri(
                    level == AlertLevelType.Critical
                        ? global::Android.Media.RingtoneType.Alarm
                        : global::Android.Media.RingtoneType.Notification);

                var ringtone = global::Android.Media.RingtoneManager.GetRingtone(_context, uri);
                ringtone?.Play();
            }
            catch
            {
                // Ignore audio errors
            }
        }).ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public async Task VibrateAsync(AlertLevelType level)
    {
        await Task.Run(() =>
        {
            try
            {
                var vibrator = (Vibrator?)_context.GetSystemService(Context.VibratorService);
                if (vibrator == null || !vibrator.HasVibrator)
                {
                    return;
                }

                long[] pattern = level switch
                {
                    AlertLevelType.Critical => new long[] { 0, 500, 200, 500, 200, 500 },
                    AlertLevelType.Warning => new long[] { 0, 300, 200, 300 },
                    _ => new long[] { 0, 200 }
                };

                if (Build.VERSION.SdkInt >= BuildVersionCodes.O)
                {
                    vibrator.Vibrate(VibrationEffect.CreateWaveform(pattern, -1));
                }
                else
                {
#pragma warning disable CS0618 // Deprecated but needed for older Android
                    vibrator.Vibrate(pattern, -1);
#pragma warning restore CS0618
                }
            }
            catch
            {
                // Ignore vibration errors
            }
        }).ConfigureAwait(false);
    }

    /// <summary>
    /// Raises the AlertResponded event.
    /// </summary>
    internal void RaiseAlertResponded(int alertId, AcknowledgmentType responseType)
    {
        AlertResponded?.Invoke(this, new AlertResponseEventArgs(alertId, responseType));
    }
}

/// <summary>
/// Broadcast receiver for alert action buttons.
/// </summary>
[BroadcastReceiver(Enabled = true, Exported = false)]
public class AlertActionReceiver : BroadcastReceiver
{
    /// <inheritdoc/>
    public override void OnReceive(Context? context, Intent? intent)
    {
        if (intent == null || context == null)
        {
            return;
        }

        var alertId = intent.GetIntExtra("AlertId", -1);
        if (alertId == -1)
        {
            return;
        }

        var action = intent.Action;
        var responseType = action switch
        {
            "HANDLED" => AcknowledgmentType.Handled,
            "SNOOZED" => AcknowledgmentType.Snoozed,
            "DISMISSED" => AcknowledgmentType.Dismissed,
            _ => AcknowledgmentType.Dismissed
        };

        // Get the notification service and raise event
        var notificationService = IPlatformApplication.Current?.Services.GetService<INotificationService>();
        if (notificationService is AndroidNotificationService androidService)
        {
            androidService.RaiseAlertResponded(alertId, responseType);
        }

        // Dismiss notification
        var notificationManager = (NotificationManager?)context.GetSystemService(Context.NotificationService);
        notificationManager?.CancelAll();
    }
}
#endif
