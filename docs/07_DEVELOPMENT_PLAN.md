# תכנית פיתוח מלאה - אפליקציית נוכחות הורית (Parental Presence App)

## 🎯 סקירת הפרויקט

**שם מוצע**: "HereForYou" / "כאן בשבילך"

**ייעוד**: אפליקציה cross-platform שעוזרת להורים להיות מודעים ונוכחים עם ילדיהם על ידי זיהוי פניות, ניטור זמן מסך, ומתן תובנות על איכות הזמן המשפחתי.

**פלטפורמות יעד**: Android, iOS, Windows, macOS

---

## 📚 סטק טכנולוגי מומלץ (מותאם ל-C# Developer)

### Frontend - .NET MAUI
**למה זה מושלם בשבילך:**
- **C# טהור** - אותה שפה שאתה מכיר
- **XAML** לממשק - דומה ל-WPF אם הכרת
- **Cross-platform אמיתי** - קוד אחד לכל הפלטפורמות
- **גישה native** לכל פיצ'רים של המכשיר
- **Community Package Ecosystem** (.NET NuGet)

### Backend (אופציונלי - אם תרצה sync בענן)
- **ASP.NET Core Web API** - הידע שלך ישירות
- **SignalR** לעדכונים בזמן אמת (למשל sync בין בני זוג)
- **SQLite** ל-local storage
- **Azure/AWS** רק אם צריך ענן (אבל ננסה להישאר local-first)

### כלי זיהוי קול
- **Picovoice Porcupine** - זיהוי wake words (יש .NET SDK!)
- **Vosk** - transcription offline (יש C# wrapper)
- **Microsoft Azure Speech SDK** - חלופה אם רוצים cloud-based (אבל פחות מומלץ לפרטיות)

### Database & Storage
- **SQLite-net-pcl** - SQLite ל-MAUI
- **Akavache** - caching reactive (אם רוצים advanced)
- **SecureStorage** מובנה ב-MAUI לסודות

### ניטור מסך
- **Android**: UsageStatsManager + AccessibilityService
- **iOS**: Screen Time API (מוגבל מאוד! נדבר על זה)
- **Windows**: Win32 APIs דרך Platform-Specific Code
- **כולם**: MAUI Community Toolkit

### State Management
- **MVVM Pattern** - נתמך מצוין ב-MAUI
- **CommunityToolkit.Mvvm** - הפשטה קלה של MVVM
- **ReactiveUI** אם אתה אוהב Reactive Programming

---

## 🏗️ ארכיטקטורה כללית

```
┌─────────────────────────────────────────────────────────┐
│                    MAUI UI Layer (XAML/C#)              │
│  ┌──────────────┐  ┌──────────────┐  ┌──────────────┐  │
│  │  Home View   │  │ Insights View│  │ Settings View│  │
│  └──────────────┘  └──────────────┘  └──────────────┘  │
└─────────────────────────────────────────────────────────┘
                           │
┌─────────────────────────────────────────────────────────┐
│              ViewModels (MVVM Pattern)                   │
│  - HomeViewModel                                         │
│  - InsightsViewModel                                     │
│  - SettingsViewModel                                     │
└─────────────────────────────────────────────────────────┘
                           │
┌─────────────────────────────────────────────────────────┐
│                   Services Layer                         │
│  ┌────────────────────┐  ┌────────────────────┐        │
│  │ Audio Monitor      │  │ Screen Time        │        │
│  │ Service            │  │ Monitor Service    │        │
│  └────────────────────┘  └────────────────────┘        │
│  ┌────────────────────┐  ┌────────────────────┐        │
│  │ Notification       │  │ Analytics          │        │
│  │ Service            │  │ Service            │        │
│  └────────────────────┘  └────────────────────┘        │
└─────────────────────────────────────────────────────────┘
                           │
┌─────────────────────────────────────────────────────────┐
│              Platform-Specific Layer                     │
│  ┌──────────────┐  ┌──────────────┐  ┌──────────────┐  │
│  │  Android     │  │     iOS      │  │   Windows    │  │
│  │  Handlers    │  │   Handlers   │  │   Handlers   │  │
│  └──────────────┘  └──────────────┘  └──────────────┘  │
└─────────────────────────────────────────────────────────┘
                           │
┌─────────────────────────────────────────────────────────┐
│            Native SDKs & APIs                            │
│  - Picovoice (voice detection)                          │
│  - Platform permissions                                  │
│  - Background services                                   │
│  - Local notifications                                   │
└─────────────────────────────────────────────────────────┘
                           │
┌─────────────────────────────────────────────────────────┐
│          Local Database (SQLite)                         │
│  - Detection events                                      │
│  - Screen time logs                                      │
│  - User settings                                         │
│  - Analytics data                                        │
└─────────────────────────────────────────────────────────┘
```

---

## 📋 תכנית פיתוח בשלבים (6 חודשים ל-MVP מלא)

### Phase 0: הכנה וסביבת פיתוח (שבוע 1)

**מטרות:**
- הקמת סביבת פיתוח
- הבנת הכלים
- פרויקט ראשון פשוט

**משימות:**
1. התקנת Visual Studio 2022 (חובה הגרסה האחרונה)
   - Workload: .NET Multi-platform App UI development
   - Android SDK
   - iOS SDK (אם יש Mac)

2. פרויקט Hello World ב-MAUI
   - יצירת פרויקט חדש: File → New → .NET MAUI App
   - הרצה על Android Emulator
   - הרצה על Windows
   - הבנת המבנה: Views, ViewModels, Services

3. למידה בסיסית:
   - Microsoft Learn: .NET MAUI Tutorials
   - הבנת MVVM Pattern
   - הבנת Dependency Injection ב-MAUI

**תוצר:** אפליקציית Hello World רצה על אמולטור

---

### Phase 1: MVP - זיהוי קול בסיסי (שבועות 2-4)

**מטרות:**
- זיהוי מילה אחת ("אמא" או "אבא")
- התראה פשוטה אחרי 3 זיהויים
- UI בסיסי

#### Week 2: הקמת התשתית

**משימות טכניות:**

**1.1 יצירת מבנה הפרויקט**
```
HereForYou/
├── HereForYou/                    # Shared Code
│   ├── Models/
│   │   ├── DetectionEvent.cs
│   │   └── AppSettings.cs
│   ├── Services/
│   │   ├── IAudioMonitorService.cs
│   │   └── INotificationService.cs
│   ├── ViewModels/
│   │   └── MainViewModel.cs
│   ├── Views/
│   │   └── MainPage.xaml
│   └── App.xaml
├── Platforms/
│   ├── Android/
│   │   ├── Services/
│   │   │   └── AndroidAudioMonitorService.cs
│   │   └── MainActivity.cs
│   ├── iOS/
│   │   └── Services/
│   │       └── iOSAudioMonitorService.cs
│   └── Windows/
└── HereForYou.csproj
```

**1.2 הגדרת Models**

`Models/DetectionEvent.cs`:
```csharp
public class DetectionEvent
{
    public int Id { get; set; }
    public DateTime Timestamp { get; set; }
    public string Keyword { get; set; }  // "אמא", "אבא"
    public float Confidence { get; set; }
    public bool WasRespondedTo { get; set; }
    public TimeSpan? ResponseTime { get; set; }
}
```

`Models/AppSettings.cs`:
```csharp
public class AppSettings
{
    public List<string> Keywords { get; set; } = new() { "אמא", "אבא" };
    public int DetectionThreshold { get; set; } = 3;  // כמה פעמים צריך לזהות
    public TimeSpan ThresholdWindow { get; set; } = TimeSpan.FromSeconds(30);
    public bool IsMonitoringEnabled { get; set; } = false;
}
```

**1.3 יצירת Services Interfaces**

`Services/IAudioMonitorService.cs`:
```csharp
public interface IAudioMonitorService
{
    Task<bool> StartMonitoringAsync();
    Task StopMonitoringAsync();
    bool IsMonitoring { get; }
    
    event EventHandler<DetectionEvent> KeywordDetected;
    event EventHandler<string> ErrorOccurred;
}
```

`Services/INotificationService.cs`:
```csharp
public interface INotificationService
{
    Task ShowAlertAsync(string title, string message, AlertLevel level);
    Task<bool> RequestPermissionsAsync();
}

public enum AlertLevel
{
    Info,
    Warning,
    Critical
}
```

#### Week 3: אינטגרציה עם Picovoice (Android בלבד בשלב זה)

**3.1 התקנת Picovoice NuGet Package**
```bash
Install-Package Picovoice.Porcupine
```

**3.2 יצירת Android Audio Monitor Service**

`Platforms/Android/Services/AndroidAudioMonitorService.cs`:
```csharp
using Picovoice;
using Android.Media;

public class AndroidAudioMonitorService : IAudioMonitorService
{
    private PorcupineManager _porcupineManager;
    private bool _isMonitoring;
    
    public bool IsMonitoring => _isMonitoring;
    
    public event EventHandler<DetectionEvent> KeywordDetected;
    public event EventHandler<string> ErrorOccurred;
    
    public async Task<bool> StartMonitoringAsync()
    {
        try
        {
            // רישום למפתח API של Picovoice (יש tier חינמי)
            string accessKey = "YOUR_PICOVOICE_ACCESS_KEY";
            
            // יצירת מילות מפתח מותאמות אישית
            // אפשר ליצור ב-Picovoice Console: https://console.picovoice.ai/
            List<string> keywordPaths = new()
            {
                "path/to/ima_android.ppn",    // קובץ מודל ל"אמא"
                "path/to/abba_android.ppn"    // קובץ מודל ל"אבא"
            };
            
            _porcupineManager = new PorcupineManager.Builder()
                .SetAccessKey(accessKey)
                .SetKeywordPaths(keywordPaths.ToArray())
                .SetSensitivities(new float[] { 0.7f, 0.7f })  // רגישות זיהוי
                .Build(
                    (keywordIndex) => OnKeywordDetected(keywordIndex)
                );
            
            _porcupineManager.Start();
            _isMonitoring = true;
            
            return true;
        }
        catch (Exception ex)
        {
            ErrorOccurred?.Invoke(this, ex.Message);
            return false;
        }
    }
    
    private void OnKeywordDetected(int keywordIndex)
    {
        string keyword = keywordIndex == 0 ? "אמא" : "אבא";
        
        var detectionEvent = new DetectionEvent
        {
            Timestamp = DateTime.Now,
            Keyword = keyword,
            Confidence = 0.9f,  // Porcupine לא מחזיר confidence, אבל אפשר להניח גבוה
            WasRespondedTo = false
        };
        
        KeywordDetected?.Invoke(this, detectionEvent);
    }
    
    public async Task StopMonitoringAsync()
    {
        _porcupineManager?.Stop();
        _porcupineManager?.Delete();
        _isMonitoring = false;
    }
}
```

**3.3 רישום ה-Service ב-Dependency Injection**

`MauiProgram.cs`:
```csharp
public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
            });
        
        // Register services
#if ANDROID
        builder.Services.AddSingleton<IAudioMonitorService, AndroidAudioMonitorService>();
#elif IOS
        builder.Services.AddSingleton<IAudioMonitorService, iOSAudioMonitorService>();
#endif
        
        builder.Services.AddSingleton<INotificationService, NotificationService>();
        builder.Services.AddTransient<MainViewModel>();
        builder.Services.AddTransient<MainPage>();
        
        return builder.Build();
    }
}
```

**3.4 הגדרת הרשאות Android**

`Platforms/Android/AndroidManifest.xml`:
```xml
<?xml version="1.0" encoding="utf-8"?>
<manifest xmlns:android="http://schemas.android.com/apk/res/android">
    <application android:allowBackup="true" android:icon="@mipmap/appicon" android:supportsRtl="true"></application>
    
    <!-- הרשאות נדרשות -->
    <uses-permission android:name="android.permission.RECORD_AUDIO" />
    <uses-permission android:name="android.permission.FOREGROUND_SERVICE" />
    <uses-permission android:name="android.permission.POST_NOTIFICATIONS" />
    <uses-permission android:name="android.permission.SYSTEM_ALERT_WINDOW" />
    
    <uses-sdk android:minSdkVersion="24" android:targetSdkVersion="33" />
</manifest>
```

#### Week 4: UI ראשוני ולוגיקת ההתראות

**4.1 יצירת MainViewModel**

`ViewModels/MainViewModel.cs`:
```csharp
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

public partial class MainViewModel : ObservableObject
{
    private readonly IAudioMonitorService _audioMonitor;
    private readonly INotificationService _notificationService;
    private readonly Queue<DetectionEvent> _recentDetections = new();
    
    [ObservableProperty]
    private bool isMonitoring;
    
    [ObservableProperty]
    private int detectionCount;
    
    [ObservableProperty]
    private string statusMessage = "לא פעיל";
    
    public MainViewModel(
        IAudioMonitorService audioMonitor,
        INotificationService notificationService)
    {
        _audioMonitor = audioMonitor;
        _notificationService = notificationService;
        
        _audioMonitor.KeywordDetected += OnKeywordDetected;
    }
    
    [RelayCommand]
    private async Task ToggleMonitoring()
    {
        if (IsMonitoring)
        {
            await _audioMonitor.StopMonitoringAsync();
            IsMonitoring = false;
            StatusMessage = "ניטור הופסק";
        }
        else
        {
            var hasPermission = await CheckAndRequestPermissions();
            if (!hasPermission)
            {
                await _notificationService.ShowAlertAsync(
                    "הרשאות חסרות",
                    "האפליקציה צריכה גישה למיקרופון",
                    AlertLevel.Warning
                );
                return;
            }
            
            var started = await _audioMonitor.StartMonitoringAsync();
            if (started)
            {
                IsMonitoring = true;
                StatusMessage = "מאזין...";
            }
        }
    }
    
    private void OnKeywordDetected(object sender, DetectionEvent e)
    {
        // הוספה לתור
        _recentDetections.Enqueue(e);
        
        // ניקוי זיהויים ישנים (מעבר לחלון הזמן)
        var threshold = DateTime.Now - TimeSpan.FromSeconds(30);
        while (_recentDetections.Count > 0 && 
               _recentDetections.Peek().Timestamp < threshold)
        {
            _recentDetections.Dequeue();
        }
        
        DetectionCount = _recentDetections.Count;
        
        // אם הגענו לסף - התראה!
        if (DetectionCount >= 3)
        {
            MainThread.BeginInvokeOnMainThread(async () =>
            {
                await _notificationService.ShowAlertAsync(
                    "הילד שלך קורא לך! 👶",
                    $"{e.Keyword} נאמר {DetectionCount} פעמים",
                    AlertLevel.Critical
                );
            });
            
            // איפוס
            _recentDetections.Clear();
            DetectionCount = 0;
        }
    }
    
    private async Task<bool> CheckAndRequestPermissions()
    {
        var status = await Permissions.CheckStatusAsync<Permissions.Microphone>();
        
        if (status != PermissionStatus.Granted)
        {
            status = await Permissions.RequestAsync<Permissions.Microphone>();
        }
        
        return status == PermissionStatus.Granted;
    }
}
```

**4.2 יצירת MainPage UI**

`Views/MainPage.xaml`:
```xml
<?xml version="1.0" encoding="utf-8" ?>
<ContentPage xmlns="http://schemas.microsoft.com/dotnet/2021/maui"
             xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml"
             xmlns:vm="clr-namespace:HereForYou.ViewModels"
             x:Class="HereForYou.MainPage"
             x:DataType="vm:MainViewModel"
             Title="כאן בשבילך">
    
    <ScrollView>
        <VerticalStackLayout Spacing="25" Padding="30">
            
            <!-- כותרת -->
            <Label Text="👂 כאן בשבילך"
                   FontSize="32"
                   FontAttributes="Bold"
                   HorizontalOptions="Center" />
            
            <!-- סטטוס -->
            <Frame BackgroundColor="{AppThemeBinding Light={StaticResource Primary}, Dark={StaticResource PrimaryDark}}"
                   CornerRadius="10"
                   Padding="20">
                <VerticalStackLayout Spacing="10">
                    <Label Text="{Binding StatusMessage}"
                           FontSize="20"
                           HorizontalOptions="Center"
                           TextColor="White" />
                    
                    <Label Text="{Binding DetectionCount, StringFormat='זיהויים אחרונים: {0}'}"
                           FontSize="16"
                           HorizontalOptions="Center"
                           TextColor="White" />
                </VerticalStackLayout>
            </Frame>
            
            <!-- כפתור הפעלה/כיבוי -->
            <Button Text="{Binding IsMonitoring, Converter={StaticResource MonitoringTextConverter}}"
                    Command="{Binding ToggleMonitoringCommand}"
                    HeightRequest="60"
                    FontSize="18"
                    BackgroundColor="{Binding IsMonitoring, Converter={StaticResource MonitoringColorConverter}}" />
            
            <!-- הסבר -->
            <Frame BackgroundColor="{AppThemeBinding Light=#F5F5F5, Dark=#2C2C2C}"
                   CornerRadius="10"
                   Padding="15">
                <Label TextType="Html">
                    <Label.Text>
                        <![CDATA[
                        <h3>איך זה עובד?</h3>
                        <p>האפליקציה מאזינה למילות המפתח:</p>
                        <ul>
                            <li>אמא</li>
                            <li>אבא</li>
                        </ul>
                        <p>כשהמילה נאמרת 3 פעמים תוך 30 שניות, תקבל התראה.</p>
                        <p><b>הפרטיות שלך מוגנת:</b> הזיהוי מתבצע במכשיר שלך בלבד.</p>
                        ]]>
                    </Label.Text>
                </Label>
            </Frame>
            
        </VerticalStackLayout>
    </ScrollView>
</ContentPage>
```

**תוצר Phase 1:**
✅ אפליקציה שמזהה "אמא" או "אבא"
✅ התראה אחרי 3 זיהויים תוך 30 שניות
✅ UI פשוט ופונקציונלי
✅ רץ על Android

---

### Phase 2: ניטור זמן מסך בסיסי (שבועות 5-7)

**מטרות:**
- מעקב אחר זמן שהאפליקציה פעילה/לא פעילה
- שמירת נתונים ב-SQLite
- תצוגת היסטוריה בסיסית

#### Week 5: הגדרת Database

**5.1 התקנת SQLite Package**
```bash
Install-Package sqlite-net-pcl
Install-Package SQLitePCLRaw.bundle_green
```

**5.2 יצירת Database Models**

`Models/ScreenTimeSession.cs`:
```csharp
using SQLite;

[Table("screen_time_sessions")]
public class ScreenTimeSession
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }
    
    [Indexed]
    public DateTime StartTime { get; set; }
    
    public DateTime? EndTime { get; set; }
    
    public TimeSpan Duration => EndTime.HasValue 
        ? EndTime.Value - StartTime 
        : DateTime.Now - StartTime;
    
    public string AppName { get; set; }  // לעתיד - איזה אפליקציה
    
    public bool WasInterrupted { get; set; }  // האם היה זיהוי בזמן השימוש
}
```

**5.3 יצירת Database Service**

`Services/DatabaseService.cs`:
```csharp
using SQLite;

public class DatabaseService
{
    private readonly SQLiteAsyncConnection _database;
    
    public DatabaseService(string dbPath)
    {
        _database = new SQLiteAsyncConnection(dbPath);
        _database.CreateTableAsync<DetectionEvent>().Wait();
        _database.CreateTableAsync<ScreenTimeSession>().Wait();
    }
    
    // Detection Events
    public Task<List<DetectionEvent>> GetDetectionEventsAsync(DateTime from, DateTime to)
    {
        return _database.Table<DetectionEvent>()
            .Where(e => e.Timestamp >= from && e.Timestamp <= to)
            .ToListAsync();
    }
    
    public Task<int> SaveDetectionEventAsync(DetectionEvent detectionEvent)
    {
        if (detectionEvent.Id != 0)
            return _database.UpdateAsync(detectionEvent);
        else
            return _database.InsertAsync(detectionEvent);
    }
    
    // Screen Time
    public Task<List<ScreenTimeSession>> GetScreenTimeSessionsAsync(DateTime date)
    {
        var startOfDay = date.Date;
        var endOfDay = startOfDay.AddDays(1);
        
        return _database.Table<ScreenTimeSession>()
            .Where(s => s.StartTime >= startOfDay && s.StartTime < endOfDay)
            .ToListAsync();
    }
    
    public Task<int> SaveScreenTimeSessionAsync(ScreenTimeSession session)
    {
        if (session.Id != 0)
            return _database.UpdateAsync(session);
        else
            return _database.InsertAsync(session);
    }
    
    public async Task<TimeSpan> GetTotalScreenTimeAsync(DateTime date)
    {
        var sessions = await GetScreenTimeSessionsAsync(date);
        return TimeSpan.FromTicks(sessions.Sum(s => s.Duration.Ticks));
    }
}
```

#### Week 6-7: אינטגרציה עם Android Usage Stats

**6.1 יצירת Screen Monitor Service - Android**

`Platforms/Android/Services/AndroidScreenMonitorService.cs`:
```csharp
using Android.App;
using Android.App.Usage;
using Android.Content;

public class AndroidScreenMonitorService : IScreenMonitorService
{
    private readonly Context _context;
    private readonly DatabaseService _database;
    private ScreenTimeSession _currentSession;
    private System.Timers.Timer _monitorTimer;
    
    public AndroidScreenMonitorService(DatabaseService database)
    {
        _database = database;
        _context = Android.App.Application.Context;
    }
    
    public async Task StartMonitoringAsync()
    {
        // בדיקת הרשאות
        if (!HasUsageStatsPermission())
        {
            RequestUsageStatsPermission();
            return;
        }
        
        // התחלת סשן חדש
        _currentSession = new ScreenTimeSession
        {
            StartTime = DateTime.Now,
            AppName = "General",
            WasInterrupted = false
        };
        
        await _database.SaveScreenTimeSessionAsync(_currentSession);
        
        // טיימר לעדכון תקופתי
        _monitorTimer = new System.Timers.Timer(60000); // כל דקה
        _monitorTimer.Elapsed += async (s, e) => await UpdateCurrentSession();
        _monitorTimer.Start();
    }
    
    public async Task StopMonitoringAsync()
    {
        _monitorTimer?.Stop();
        
        if (_currentSession != null)
        {
            _currentSession.EndTime = DateTime.Now;
            await _database.SaveScreenTimeSessionAsync(_currentSession);
            _currentSession = null;
        }
    }
    
    private async Task UpdateCurrentSession()
    {
        if (_currentSession != null)
        {
            await _database.SaveScreenTimeSessionAsync(_currentSession);
        }
    }
    
    private bool HasUsageStatsPermission()
    {
        var usageStatsManager = (UsageStatsManager)_context
            .GetSystemService(Context.UsageStatsService);
        
        var endTime = DateTimeOffset.Now.ToUnixTimeMilliseconds();
        var startTime = endTime - 1000 * 60; // דקה אחרונה
        
        var queryUsageStats = usageStatsManager.QueryUsageStats(
            UsageStatsInterval.Daily, 
            startTime, 
            endTime
        );
        
        return queryUsageStats != null && queryUsageStats.Count > 0;
    }
    
    private void RequestUsageStatsPermission()
    {
        var intent = new Intent(Android.Provider.Settings.ActionUsageAccessSettings);
        intent.SetFlags(ActivityFlags.NewTask);
        _context.StartActivity(intent);
    }
    
    public async Task<TimeSpan> GetTodayScreenTimeAsync()
    {
        return await _database.GetTotalScreenTimeAsync(DateTime.Today);
    }
}
```

**תוצר Phase 2:**
✅ מעקב אחר זמן שימוש במכשיר
✅ שמירה ב-SQLite
✅ API לשאילתת נתונים

---

### Phase 3: תובנות וויזואליזציה (שבועות 8-10)

**מטרות:**
- דף Insights עם גרפים
- סטטיסטיקות יומיות/שבועיות
- "אינדקס נוכחות"

#### Week 8: יצירת Insights ViewModel

`ViewModels/InsightsViewModel.cs`:
```csharp
using CommunityToolkit.Mvvm.ComponentModel;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;

public partial class InsightsViewModel : ObservableObject
{
    private readonly DatabaseService _database;
    
    [ObservableProperty]
    private TimeSpan todayScreenTime;
    
    [ObservableProperty]
    private int todayDetections;
    
    [ObservableProperty]
    private int respondedDetections;
    
    [ObservableProperty]
    private double presenceScore;  // 0-100
    
    [ObservableProperty]
    private ISeries[] screenTimeSeries;
    
    public InsightsViewModel(DatabaseService database)
    {
        _database = database;
    }
    
    public async Task LoadDataAsync()
    {
        // נתוני היום
        TodayScreenTime = await _database.GetTotalScreenTimeAsync(DateTime.Today);
        
        var todayDetectionEvents = await _database.GetDetectionEventsAsync(
            DateTime.Today, 
            DateTime.Now
        );
        
        TodayDetections = todayDetectionEvents.Count;
        RespondedDetections = todayDetectionEvents.Count(d => d.WasRespondedTo);
        
        // חישוב Presence Score
        PresenceScore = CalculatePresenceScore(TodayScreenTime, todayDetectionEvents);
        
        // נתונים לגרף שבועי
        await LoadWeeklyChartData();
    }
    
    private double CalculatePresenceScore(
        TimeSpan screenTime, 
        List<DetectionEvent> detections)
    {
        // לוגיקה פשוטה:
        // 100 נקודות בסיס
        // -10 נקודות לכל שעת מסך מעל 4 שעות
        // -5 נקודות לכל זיהוי לא מטופל
        
        double score = 100;
        
        var excessHours = Math.Max(0, screenTime.TotalHours - 4);
        score -= excessHours * 10;
        
        var unanswered = detections.Count(d => !d.WasRespondedTo);
        score -= unanswered * 5;
        
        return Math.Max(0, Math.Min(100, score));
    }
    
    private async Task LoadWeeklyChartData()
    {
        var weekData = new List<double>();
        
        for (int i = 6; i >= 0; i--)
        {
            var date = DateTime.Today.AddDays(-i);
            var screenTime = await _database.GetTotalScreenTimeAsync(date);
            weekData.Add(screenTime.TotalHours);
        }
        
        ScreenTimeSeries = new ISeries[]
        {
            new ColumnSeries<double>
            {
                Name = "שעות מסך",
                Values = weekData,
                Fill = new SolidColorPaint(SKColors.CornflowerBlue)
            }
        };
    }
}
```

#### Week 9-10: בניית Insights UI

`Views/InsightsPage.xaml`:
```xml
<?xml version="1.0" encoding="utf-8" ?>
<ContentPage xmlns="http://schemas.microsoft.com/dotnet/2021/maui"
             xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml"
             xmlns:vm="clr-namespace:HereForYou.ViewModels"
             xmlns:lvc="clr-namespace:LiveChartsCore.SkiaSharpView.Maui;assembly=LiveChartsCore.SkiaSharpView.Maui"
             x:Class="HereForYou.Views.InsightsPage"
             x:DataType="vm:InsightsViewModel"
             Title="תובנות">
    
    <ScrollView>
        <VerticalStackLayout Spacing="20" Padding="20">
            
            <!-- Presence Score -->
            <Frame BackgroundColor="{StaticResource Primary}" 
                   CornerRadius="15" 
                   Padding="20">
                <VerticalStackLayout Spacing="10">
                    <Label Text="מדד הנוכחות שלך" 
                           FontSize="18" 
                           TextColor="White"
                           HorizontalOptions="Center" />
                    
                    <Label Text="{Binding PresenceScore, StringFormat='{0:F0}'}" 
                           FontSize="48" 
                           FontAttributes="Bold"
                           TextColor="White"
                           HorizontalOptions="Center" />
                    
                    <ProgressBar Progress="{Binding PresenceScore, Converter={StaticResource ScoreToProgressConverter}}"
                                 ProgressColor="White" />
                    
                    <Label Text="{Binding PresenceScore, Converter={StaticResource ScoreToTextConverter}}"
                           FontSize="14"
                           TextColor="White"
                           HorizontalOptions="Center" />
                </VerticalStackLayout>
            </Frame>
            
            <!-- סטטיסטיקות היום -->
            <Grid ColumnDefinitions="*,*" RowDefinitions="Auto,Auto" ColumnSpacing="10" RowSpacing="10">
                
                <!-- זמן מסך -->
                <Frame Grid.Row="0" Grid.Column="0" BackgroundColor="#E3F2FD">
                    <VerticalStackLayout Spacing="5">
                        <Label Text="📱" FontSize="32" HorizontalOptions="Center" />
                        <Label Text="{Binding TodayScreenTime, StringFormat='{0:hh\\:mm}'}"
                               FontSize="24"
                               FontAttributes="Bold"
                               HorizontalOptions="Center" />
                        <Label Text="זמן מסך היום"
                               FontSize="12"
                               HorizontalOptions="Center" />
                    </VerticalStackLayout>
                </Frame>
                
                <!-- זיהויים -->
                <Frame Grid.Row="0" Grid.Column="1" BackgroundColor="#FFF3E0">
                    <VerticalStackLayout Spacing="5">
                        <Label Text="👂" FontSize="32" HorizontalOptions="Center" />
                        <Label Text="{Binding TodayDetections}"
                               FontSize="24"
                               FontAttributes="Bold"
                               HorizontalOptions="Center" />
                        <Label Text="פניות היום"
                               FontSize="12"
                               HorizontalOptions="Center" />
                    </VerticalStackLayout>
                </Frame>
                
                <!-- תגובות -->
                <Frame Grid.Row="1" Grid.Column="0" BackgroundColor="#E8F5E9">
                    <VerticalStackLayout Spacing="5">
                        <Label Text="✅" FontSize="32" HorizontalOptions="Center" />
                        <Label Text="{Binding RespondedDetections}"
                               FontSize="24"
                               FontAttributes="Bold"
                               HorizontalOptions="Center" />
                        <Label Text="טופלו"
                               FontSize="12"
                               HorizontalOptions="Center" />
                    </VerticalStackLayout>
                </Frame>
                
                <!-- אחוז היענות -->
                <Frame Grid.Row="1" Grid.Column="1" BackgroundColor="#FCE4EC">
                    <VerticalStackLayout Spacing="5">
                        <Label Text="📊" FontSize="32" HorizontalOptions="Center" />
                        <Label Text="{Binding RespondedDetections, Converter={StaticResource ResponseRateConverter}}"
                               FontSize="24"
                               FontAttributes="Bold"
                               HorizontalOptions="Center" />
                        <Label Text="אחוז היענות"
                               FontSize="12"
                               HorizontalOptions="Center" />
                    </VerticalStackLayout>
                </Frame>
                
            </Grid>
            
            <!-- גרף שבועי -->
            <Frame CornerRadius="10" Padding="10">
                <VerticalStackLayout Spacing="10">
                    <Label Text="זמן מסך - 7 ימים אחרונים"
                           FontSize="16"
                           FontAttributes="Bold" />
                    
                    <lvc:CartesianChart Series="{Binding ScreenTimeSeries}"
                                        HeightRequest="250" />
                </VerticalStackLayout>
            </Frame>
            
        </VerticalStackLayout>
    </ScrollView>
</ContentPage>
```

**תוצר Phase 3:**
✅ דף תובנות מלא
✅ מדד נוכחות
✅ גרפים ויזואליים
✅ סטטיסטיקות מפורטות

---

### Phase 4: התראות מתקדמות וריצה ברקע (שבועות 11-13)

**מטרות:**
- Foreground Service ל-Android
- התראות overlay מסך מלא
- רמות התראה מדורגות

#### Week 11: Android Foreground Service

**11.1 יצירת Foreground Service**

`Platforms/Android/Services/MonitoringForegroundService.cs`:
```csharp
using Android.App;
using Android.Content;
using Android.OS;
using AndroidX.Core.App;

[Service(ForegroundServiceType = Android.Content.PM.ForegroundService.TypeMicrophone)]
public class MonitoringForegroundService : Service
{
    private const int ServiceNotificationId = 1000;
    private const string ChannelId = "monitoring_channel";
    
    private IAudioMonitorService _audioMonitor;
    private IScreenMonitorService _screenMonitor;
    
    public override IBinder OnBind(Intent intent) => null;
    
    public override StartCommandResult OnStartCommand(Intent intent, StartCommandFlags flags, int startId)
    {
        CreateNotificationChannel();
        
        var notification = new NotificationCompat.Builder(this, ChannelId)
            .SetContentTitle("כאן בשבילך פעיל")
            .SetContentText("מאזין לפניות הילדים שלך")
            .SetSmallIcon(Resource.Drawable.ic_notification)
            .SetOngoing(true)
            .SetPriority(NotificationCompat.PriorityLow)
            .Build();
        
        StartForeground(ServiceNotificationId, notification);
        
        // אתחול services
        _audioMonitor = IPlatformApplication.Current.Services
            .GetService<IAudioMonitorService>();
        _screenMonitor = IPlatformApplication.Current.Services
            .GetService<IScreenMonitorService>();
        
        Task.Run(async () =>
        {
            await _audioMonitor.StartMonitoringAsync();
            await _screenMonitor.StartMonitoringAsync();
        });
        
        return StartCommandResult.Sticky;
    }
    
    public override void OnDestroy()
    {
        Task.Run(async () =>
        {
            await _audioMonitor?.StopMonitoringAsync();
            await _screenMonitor?.StopMonitoringAsync();
        });
        
        base.OnDestroy();
    }
    
    private void CreateNotificationChannel()
    {
        if (Build.VERSION.SdkInt >= BuildVersionCodes.O)
        {
            var channel = new NotificationChannel(
                ChannelId,
                "ניטור רקע",
                NotificationImportance.Low
            )
            {
                Description = "מאפשר לאפליקציה לרוץ ברקע"
            };
            
            var notificationManager = (NotificationManager)GetSystemService(NotificationService);
            notificationManager.CreateNotificationChannel(channel);
        }
    }
}
```

#### Week 12-13: Overlay Notifications

**12.1 יצירת Overlay Window Service**

`Platforms/Android/Services/OverlayNotificationService.cs`:
```csharp
using Android.Graphics;
using Android.Views;
using Android.Widget;

public class OverlayNotificationService
{
    private readonly Context _context;
    private IWindowManager _windowManager;
    private View _overlayView;
    private bool _isShowing;
    
    public OverlayNotificationService()
    {
        _context = Android.App.Application.Context;
        _windowManager = _context.GetSystemService(Context.WindowService)
            .JavaCast<IWindowManager>();
    }
    
    public void ShowFullScreenAlert(string title, string message, AlertLevel level)
    {
        if (_isShowing) return;
        
        // בדיקת הרשאה
        if (!Android.Provider.Settings.CanDrawOverlays(_context))
        {
            RequestOverlayPermission();
            return;
        }
        
        MainThread.BeginInvokeOnMainThread(() =>
        {
            CreateOverlayView(title, message, level);
            _isShowing = true;
        });
    }
    
    private void CreateOverlayView(string title, string message, AlertLevel level)
    {
        // פרמטרים לחלון
        var layoutParams = new WindowManagerLayoutParams(
            WindowManagerLayoutParams.MatchParent,
            WindowManagerLayoutParams.MatchParent,
            Build.VERSION.SdkInt >= BuildVersionCodes.O
                ? WindowManagerTypes.ApplicationOverlay
                : WindowManagerTypes.Phone,
            WindowManagerFlags.NotFocusable | 
            WindowManagerFlags.LayoutInScreen |
            WindowManagerFlags.KeepScreenOn,
            Format.Translucent
        );
        
        // יצירת Layout
        var linearLayout = new LinearLayout(_context)
        {
            Orientation = Orientation.Vertical,
            LayoutParameters = new ViewGroup.LayoutParams(
                ViewGroup.LayoutParams.MatchParent,
                ViewGroup.LayoutParams.MatchParent
            )
        };
        
        linearLayout.SetBackgroundColor(level == AlertLevel.Critical 
            ? Color.Argb(230, 244, 67, 54)   // אדום שקוף
            : Color.Argb(230, 255, 152, 0)); // כתום שקוף
        
        linearLayout.SetGravity(GravityFlags.Center);
        linearLayout.SetPadding(40, 40, 40, 40);
        
        // טקסט כותרת
        var titleView = new TextView(_context)
        {
            Text = title,
            TextSize = 32,
            Gravity = GravityFlags.Center
        };
        titleView.SetTextColor(Color.White);
        titleView.SetTypeface(null, TypefaceStyle.Bold);
        
        // טקסט הודעה
        var messageView = new TextView(_context)
        {
            Text = message,
            TextSize = 24,
            Gravity = GravityFlags.Center
        };
        messageView.SetTextColor(Color.White);
        messageView.SetPadding(0, 20, 0, 40);
        
        // כפתורים
        var buttonLayout = new LinearLayout(_context)
        {
            Orientation = Orientation.Horizontal
        };
        
        var dismissButton = new Button(_context)
        {
            Text = "אני מטפל בזה",
            LayoutParameters = new LinearLayout.LayoutParams(
                0,
                ViewGroup.LayoutParams.WrapContent,
                1.0f
            )
        };
        dismissButton.Click += (s, e) => DismissOverlay(true);
        
        var snoozeButton = new Button(_context)
        {
            Text = "עוד רגע",
            LayoutParameters = new LinearLayout.LayoutParams(
                0,
                ViewGroup.LayoutParams.WrapContent,
                1.0f
            )
        };
        snoozeButton.Click += (s, e) => DismissOverlay(false);
        
        buttonLayout.AddView(dismissButton);
        buttonLayout.AddView(snoozeButton);
        
        linearLayout.AddView(titleView);
        linearLayout.AddView(messageView);
        linearLayout.AddView(buttonLayout);
        
        _overlayView = linearLayout;
        _windowManager.AddView(_overlayView, layoutParams);
    }
    
    private void DismissOverlay(bool handled)
    {
        if (_overlayView != null)
        {
            _windowManager.RemoveView(_overlayView);
            _overlayView = null;
            _isShowing = false;
        }
        
        // TODO: עדכן ב-database אם טופל או לא
    }
    
    private void RequestOverlayPermission()
    {
        var intent = new Intent(
            Android.Provider.Settings.ActionManageOverlayPermission,
            Android.Net.Uri.Parse($"package:{_context.PackageName}")
        );
        intent.SetFlags(ActivityFlags.NewTask);
        _context.StartActivity(intent);
    }
}
```

**תוצר Phase 4:**
✅ Foreground service רץ ברקע
✅ Overlay notifications מסך מלא
✅ כפתורים "טיפלתי" / "רגע"
✅ ההרשאות הנדרשות

---

### Phase 5: הגדרות מתקדמות וכיולים (שבועות 14-16)

**מטרות:**
- אימון מילות מפתח מותאמות אישית
- הגדרות סף זיהוי
- פרופילים למשפחה

**תוצר Phase 5:**
✅ הגדרות מתקדמות
✅ כיול רגישות
✅ תמיכה במספר ילדים

---

### Phase 6: iOS Support ופיצ'רים נוספים (שבועות 17-20)

**מטרות:**
- פיתוח גרסת iOS
- זיהוי נוכחות (camera)
- Co-parenting sync

**תוצר Phase 6:**
✅ תמיכה ב-iOS
✅ Sync בין מכשירים (P2P)
✅ זיהוי פיזי בחדר

---

### Phase 7: בדיקות, ליטוש ופרסום (שבועות 21-24)

**מטרות:**
- Unit tests
- Beta testing עם משפחות אמיתיות
- אופטימיזציית סוללה
- פרסום ל-Play Store / App Store

---

## 📊 מילסטונים ומדדי הצלחה

### Milestone 1 (סוף חודש 1)
- ✅ אפליקציה רצה על Android
- ✅ מזהה מילה אחת
- ✅ מראה התראה

### Milestone 2 (סוף חודש 2)
- ✅ ניטור זמן מסך
- ✅ שמירת היסטוריה
- ✅ 3 משתמשי beta בודקים

### Milestone 3 (סוף חודש 3)
- ✅ דף תובנות מלא
- ✅ Foreground service יציב
- ✅ 10 משתמשי beta בודקים

### Milestone 4 (סוף חודש 4)
- ✅ הגדרות מלאות
- ✅ אימון מילות מפתח מותאמות
- ✅ 50 משתמשי beta

### Milestone 5 (סוף חודש 5)
- ✅ גרסת iOS
- ✅ Sync בין מכשירים
- ✅ 100+ משתמשים

### Milestone 6 (סוף חודש 6)
- ✅ פרסום ב-Play Store
- ✅ App Store review
- ✅ דוקומנטציה מלאה

---

## 🛠️ כלים ומשאבים

### למידה
- **Microsoft Learn**: .NET MAUI Tutorials
- **YouTube**: James Montemagno - .NET MAUI Playlist
- **GitHub**: microsoft/dotnet-maui-samples
- **Picovoice Docs**: https://picovoice.ai/docs/

### Packages מרכזיים
```xml
<!-- .csproj -->
<ItemGroup>
    <!-- Core -->
    <PackageReference Include="CommunityToolkit.Mvvm" Version="8.2.2" />
    <PackageReference Include="CommunityToolkit.Maui" Version="7.0.0" />
    
    <!-- Database -->
    <PackageReference Include="sqlite-net-pcl" Version="1.9.172" />
    <PackageReference Include="SQLitePCLRaw.bundle_green" Version="2.1.8" />
    
    <!-- Voice Detection -->
    <PackageReference Include="Picovoice.Porcupine" Version="3.0.0" />
    
    <!-- Charts -->
    <PackageReference Include="LiveChartsCore.SkiaSharpView.Maui" Version="2.0.0-rc2" />
    
    <!-- Permissions -->
    <PackageReference Include="Microsoft.Maui.Essentials" Version="8.0.0" />
</ItemGroup>
```

### טיפים לפיתוח
1. **התחל עם Android**: iOS מסובך יותר עם הרשאות
2. **השתמש ב-Hot Reload**: חוסך המון זמן
3. **בדוק על מכשיר אמיתי**: Emulator לא מדויק למיקרופון
4. **לוג הכל**: Debug.WriteLine הוא החבר שלך
5. **שמור קוד shared**: כל מה שאפשר ב-Shared Project

---

## 💰 אומדן עלויות

### פיתוח (בהנחה שאתה עושה לבד)
- **זמן**: 6 חודשים, ~20 שעות/שבוע = 480 שעות
- **עלות זמן** (אם היית מחשב $50/שעה): $24,000

### כלים ושירותים
- **Visual Studio**: חינם (Community Edition)
- **Picovoice Free Tier**: חינם ל-3 מילות מפתח מותאמות
- **Azure** (אופציונלי): $0-50/חודש (אם צריך backend)
- **Google Play Developer**: $25 חד פעמי
- **Apple Developer**: $99/שנה
- **שמירת דומיין**: ~$15/שנה

**סה"כ שנה ראשונה**: ~$200-300

### Scale (אם מתכנן למסחר)
- **Picovoice Enterprise**: משתנה לפי שימוש
- **Backend hosting**: $50-500/חודש תלוי בכמות משתמשים
- **משפטי/חברה**: $1,000-5,000

---

## ⚠️ אתגרים צפויים

### 1. iOS Limitations
**בעיה**: iOS לא מאפשרת ריצה ברקע כמו Android
**פתרון**: 
- Background Audio mode (להתחזות ל"נגן")
- Focus Modes + Shortcuts
- Widget שמתעדכן

### 2. Battery Drain
**בעיה**: ניטור מיקרופון מתמיד מרוקן סוללה
**פתרון**:
- Voice Activity Detection (VAD) קודם
- דגימה חכמה (לא רציפה)
- Sleep modes כשמסך כבוי

### 3. False Positives
**בעיה**: זיהוי מילים בטעות (טלוויזיה, רדיו)
**פתרון**:
- Speaker identification (זיהוי דובר ספציפי)
- Context awareness (זמן, מיקום)
- הגדרות רגישות

### 4. Privacy Concerns
**בעיה**: הורים יפחדו מהקלטה
**פתרון**:
- 100% on-device processing
- אין שמירת אודיו
- קוד פתוח (אופציה)
- דוקומנטציה ברורה

---

## 🚀 לאחר ה-MVP - כיוונים עתידיים

1. **AI Emotion Detection**: זיהוי מצוקה/כעס בקול
2. **Smart Home Integration**: עמעום אורות כשמזהה פניה
3. **Wearable Support**: התראות ל-smartwatch
4. **Family Dashboard**: תצוגה משותפת לשני הורים
5. **Gamification**: נקודות והישגים למשפחה
6. **תמיכת קהילה**: שיתוף טיפים בין הורים
7. **אינטגרציית יומן**: Notion, Google Calendar
8. **AI Coach**: המלצות מותאמות אישית

---

## 📝 הערות חשובות לפני שמתחילים

### מה כדאי לדעת מראש:
1. **C# שאתה מכיר != C# ב-MAUI**: יש ניואנסים (async/await, lifecycle)
2. **XAML לוקח זמן**: אבל יש Designer ו-Hot Reload
3. **Android Emulator איטי**: השקע במכשיר אמיתי לפיתוח
4. **הרשאות מעצבנות**: הכן סבלנות לניפוי באגים
5. **Community עוזרת**: Stack Overflow + Discord של MAUI

### מה שיעזור לך (רקע ב-C#):
✅ LINQ - משתמשים הרבה
✅ async/await - קריטי
✅ Dependency Injection - מובנה ב-MAUI
✅ Events & Delegates - לכל מקום
✅ MVVM Pattern - הבסיס של הכל

---

## 🎓 תכנית למידה מומלצת (לפני שמתחילים לקודד)

### שבוע 0 - Pre-learning
**יום 1-2**: .NET MAUI Basics
- [Microsoft Learn - Build mobile and desktop apps with .NET MAUI](https://learn.microsoft.com/training/paths/build-apps-with-dotnet-maui/)

**יום 3-4**: MVVM Pattern
- [James Montemagno - MVVM Tutorial](https://www.youtube.com/watch?v=Pso1MeX_HvI)

**יום 5-7**: הקמת פרויקט ראשון
- עקוב אחרי tutorial מלא
- צור אפליקציית TODO פשוטה

---

זו תכנית מפורטת מאוד, אבל אפשר להתאים אותה. 

**השאלות שלי אליך:**
1. האם יש לך Mac? (משפיע על פיתוח iOS)
2. יש לך מכשיר Android לבדיקות?
3. כמה זמן בשבוע אתה יכול להשקיע?
4. האם אתה מעוניין בגרסה מסחרית או רק לשימוש אישי?

**בואו נתחיל ביחד - רוצה שאני:**
1. אכין לך את ה-boilerplate code לפרויקט?
2. אכתוב tutorial מפורט ל-Phase 0?
3. נבנה יחד את ה-MVP הראשון?

מה אתה אומר?
