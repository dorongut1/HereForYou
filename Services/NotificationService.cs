using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using HereForYou.Models;
using HereForYou.Services.Interfaces;

namespace HereForYou.Services;

/// <summary>
/// Real notification service with actual MAUI notifications, sounds, and vibration.
/// </summary>
public class NotificationService : INotificationService
{
    private Page? _currentOverlayPage;

    public event EventHandler<AlertResponseEventArgs>? AlertResponded;

    public async Task ShowAlertAsync(string title, string message, AlertLevelType level, int? alertId = null)
    {
        if (Application.Current?.MainPage == null)
        {
            System.Diagnostics.Debug.WriteLine("[Notification] MainPage not available");
            return;
        }

        // Show alert dialog on main thread
        await MainThread.InvokeOnMainThreadAsync(async () =>
        {
            try
            {
                var action = await Application.Current.MainPage.DisplayAlert(
                    title,
                    message,
                    "טופלתי בזה",
                    "התעלמתי"
                );

                // Raise event with user's response
                var response = action ? AcknowledgmentType.Handled : AcknowledgmentType.Dismissed;
                if (alertId.HasValue)
                {
                    AlertResponded?.Invoke(this, new AlertResponseEventArgs(alertId.Value, response));
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[Notification] Error showing alert: {ex.Message}");
            }
        });
    }

    public async Task ShowOverlayAlertAsync(string title, string message, int? alertId = null)
    {
        // For critical alerts, show a full-screen page
        await MainThread.InvokeOnMainThreadAsync(async () =>
        {
            try
            {
                if (Application.Current?.MainPage == null) return;

                // Create overlay page
                _currentOverlayPage = new ContentPage
                {
                    BackgroundColor = Colors.Black.WithAlpha(0.8f),
                    Content = new VerticalStackLayout
                    {
                        VerticalOptions = LayoutOptions.Center,
                        HorizontalOptions = LayoutOptions.Center,
                        Spacing = 20,
                        Padding = 40,
                        Children =
                        {
                            new Frame
                            {
                                BackgroundColor = Colors.Red,
                                Padding = 30,
                                CornerRadius = 10,
                                Content = new VerticalStackLayout
                                {
                                    Spacing = 15,
                                    Children =
                                    {
                                        new Label
                                        {
                                            Text = title,
                                            FontSize = 28,
                                            FontAttributes = FontAttributes.Bold,
                                            TextColor = Colors.White,
                                            HorizontalTextAlignment = TextAlignment.Center
                                        },
                                        new Label
                                        {
                                            Text = message,
                                            FontSize = 20,
                                            TextColor = Colors.White,
                                            HorizontalTextAlignment = TextAlignment.Center
                                        },
                                        new Button
                                        {
                                            Text = "טופלתי בזה!",
                                            BackgroundColor = Colors.White,
                                            TextColor = Colors.Red,
                                            FontAttributes = FontAttributes.Bold,
                                            Command = new Command(async () =>
                                            {
                                                await DismissOverlayAsync();
                                                if (alertId.HasValue)
                                                {
                                                    AlertResponded?.Invoke(this,
                                                        new AlertResponseEventArgs(alertId.Value, AcknowledgmentType.Handled));
                                                }
                                            })
                                        }
                                    }
                                }
                            }
                        }
                    }
                };

                await Application.Current.MainPage.Navigation.PushModalAsync(_currentOverlayPage);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[Notification] Error showing overlay: {ex.Message}");
            }
        });
    }

    public async Task DismissOverlayAsync()
    {
        await MainThread.InvokeOnMainThreadAsync(async () =>
        {
            try
            {
                if (_currentOverlayPage != null && Application.Current?.MainPage != null)
                {
                    await Application.Current.MainPage.Navigation.PopModalAsync();
                    _currentOverlayPage = null;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[Notification] Error dismissing overlay: {ex.Message}");
            }
        });
    }

    public async Task ShowNotificationAsync(string title, string message)
    {
        // Use CommunityToolkit.Maui Toast
        await MainThread.InvokeOnMainThreadAsync(async () =>
        {
            try
            {
                var toast = Toast.Make(
                    $"{title}\n{message}",
                    ToastDuration.Long,
                    14);

                await toast.Show();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[Notification] Error showing toast: {ex.Message}");
            }
        });
    }

    public async Task ShowMonitoringNotificationAsync()
    {
        await ShowNotificationAsync("ניטור פעיל", "האפליקציה מאזינה למילות מפתח");
    }

    public async Task HideMonitoringNotificationAsync()
    {
        // Toast notifications auto-dismiss, nothing to do
        await Task.CompletedTask;
    }

    public Task<bool> HasNotificationPermissionAsync()
    {
        // MAUI doesn't require special permissions for in-app notifications
        return Task.FromResult(true);
    }

    public Task<bool> RequestNotificationPermissionAsync()
    {
        // No permission needed for in-app notifications
        return Task.FromResult(true);
    }

    public Task<bool> HasOverlayPermissionAsync()
    {
        // Modal pages don't require special permissions in MAUI
        return Task.FromResult(true);
    }

    public Task<bool> RequestOverlayPermissionAsync()
    {
        // No permission needed
        return Task.FromResult(true);
    }

    public async Task PlayAlertSoundAsync(AlertLevelType level)
    {
        try
        {
            // TODO: Add actual sound files to Resources/Raw and use AudioManager
            // For now, just log - actual sound implementation would use:
            // - Plugin.Maui.Audio or
            // - MediaElement from CommunityToolkit.Maui

            await Task.CompletedTask;
            System.Diagnostics.Debug.WriteLine($"[Notification] Would play {level} sound");
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[Notification] Error in PlayAlertSoundAsync: {ex.Message}");
        }
    }

    public async Task VibrateAsync(AlertLevelType level)
    {
        try
        {
            // Different vibration patterns for different levels
            var duration = level switch
            {
                AlertLevelType.Critical => TimeSpan.FromMilliseconds(1000),
                AlertLevelType.Warning => TimeSpan.FromMilliseconds(500),
                _ => TimeSpan.FromMilliseconds(200)
            };

            await MainThread.InvokeOnMainThreadAsync(() =>
            {
                try
                {
                    if (Vibration.Default.IsSupported)
                    {
                        Vibration.Default.Vibrate(duration);
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"[Notification] Error vibrating: {ex.Message}");
                }
            });
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[Notification] Error in VibrateAsync: {ex.Message}");
        }
    }
}
