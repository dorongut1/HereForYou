# Technical Stack - HereForYou
## כל הטכנולוגיות והחלטות ארכיטקטוניות

---

## 🏗️ Architecture Overview

```
┌─────────────────────────────────────────────────┐
│           Presentation Layer (MAUI)              │
│  ┌──────────┐  ┌──────────┐  ┌──────────┐      │
│  │   XAML   │  │ViewModels│  │  Views   │      │
│  └──────────┘  └──────────┘  └──────────┘      │
└─────────────────────────────────────────────────┘
                      │
┌─────────────────────────────────────────────────┐
│              Business Logic Layer                │
│  ┌──────────────────┐  ┌──────────────────┐    │
│  │ Audio Monitor    │  │ Screen Monitor   │    │
│  │ Service          │  │ Service          │    │
│  └──────────────────┘  └──────────────────┘    │
│  ┌──────────────────┐  ┌──────────────────┐    │
│  │ Notification     │  │ Analytics        │    │
│  │ Service          │  │ Service          │    │
│  └──────────────────┘  └──────────────────┘    │
└─────────────────────────────────────────────────┘
                      │
┌─────────────────────────────────────────────────┐
│               Data Access Layer                  │
│           ┌──────────────────────┐               │
│           │  DatabaseService     │               │
│           └──────────────────────┘               │
└─────────────────────────────────────────────────┘
                      │
┌─────────────────────────────────────────────────┐
│              SQLite Database                     │
│  (Local, On-Device, Encrypted)                   │
└─────────────────────────────────────────────────┘
```

---

## 💻 Core Technologies

### .NET MAUI (.NET 8)
**למה בחרנו:**
- ✅ C# - השפה שדורון מכיר
- ✅ Cross-platform native - Android, iOS, Windows, macOS
- ✅ Single codebase
- ✅ גישה מלאה ל-platform APIs
- ✅ Modern, supported, growing ecosystem

**Alternatives considered:**
- ❌ Flutter - צריך ללמוד Dart
- ❌ React Native - לא native אמיתי
- ❌ Xamarin - deprecated

### SQLite
**למה בחרנו:**
- ✅ Local-first - כל הנתונים במכשיר
- ✅ Offline - עובד בלי אינטרנט
- ✅ Privacy - אין שליחה לשרת
- ✅ Performance - מהיר וקל
- ✅ Zero configuration

**Alternatives considered:**
- ❌ SQL Server - דורש שרת, לא local
- ❌ Realm - יותר מורכב מהצורך
- ❌ LiteDB - פחות mature

### Picovoice Porcupine
**למה בחרנו:**
- ✅ On-device processing - פרטיות מלאה
- ✅ יש .NET SDK - אינטגרציה קלה
- ✅ Custom wake words - יכולים להכשיר "אמא", "אבא"
- ✅ Proven - משמשת חברות גדולות
- ✅ Free tier - עד 3 מילות מפתח

**Alternatives considered:**
- ❌ Google Speech - דורש cloud, פחות פרטי
- ❌ Azure Speech - יקר, cloud-based
- ❌ Vosk - פחות accurate למילים בעברית

---

## 📦 NuGet Packages

### Core Packages
```xml
<PackageReference Include="Microsoft.Maui.Controls" Version="8.0.*" />
<PackageReference Include="Microsoft.Maui.Controls.Compatibility" Version="8.0.*" />
```

### Database
```xml
<PackageReference Include="sqlite-net-pcl" Version="1.9.172" />
<PackageReference Include="SQLitePCLRaw.bundle_green" Version="2.1.8" />
```

### MVVM & Helpers
```xml
<PackageReference Include="CommunityToolkit.Mvvm" Version="8.2.2" />
<PackageReference Include="CommunityToolkit.Maui" Version="7.0.0" />
```

### Voice Detection
```xml
<PackageReference Include="Picovoice.Porcupine" Version="3.0.0" />
```

### Charts (for Insights)
```xml
<PackageReference Include="LiveChartsCore.SkiaSharpView.Maui" Version="2.0.0-rc2" />
```

---

## 🎨 Design Patterns

### MVVM (Model-View-ViewModel)
```
View (XAML)
  ↓ Binding
ViewModel
  ↓ Commands/Properties
Model/Services
  ↓
Data
```

**Implementation:**
- `CommunityToolkit.Mvvm` לgeneration אוטומטי
- `[ObservableProperty]` במקום boilerplate
- `[RelayCommand]` לcommands
- `INotifyPropertyChanged` אוטומטי

### Repository Pattern
```csharp
IRepository<T>
  ├─ GetByIdAsync(int id)
  ├─ GetAllAsync()
  ├─ AddAsync(T entity)
  ├─ UpdateAsync(T entity)
  └─ DeleteAsync(int id)
```

### Dependency Injection
```csharp
builder.Services.AddSingleton<IDatabaseService, DatabaseService>();
builder.Services.AddTransient<IViewModel, ViewModel>();
```

---

## 🔧 Platform-Specific Code

### Android
```
Platforms/Android/
├─ MainActivity.cs
├─ AndroidManifest.xml
└─ Services/
   ├─ AndroidAudioMonitorService.cs
   ├─ AndroidScreenMonitorService.cs
   └─ OverlayNotificationService.cs
```

**Key Android Features:**
- Foreground Service (ריצה ברקע)
- UsageStatsManager (זמן מסך)
- SYSTEM_ALERT_WINDOW (overlay)
- Audio permissions

### iOS
```
Platforms/iOS/
├─ AppDelegate.cs
├─ Info.plist
└─ Services/
   ├─ iOSAudioMonitorService.cs
   └─ iOSScreenMonitorService.cs
```

**iOS Limitations:**
- אין overlay אמיתי - רק notifications
- ריצה ברקע מוגבלת - Background Audio mode
- Screen Time API מוגבל מאוד

---

## 🗄️ Database Design

### Schema
- **8 טבלאות** עיקריות
- **SQLite** on-device
- **Triggers** לעדכונים אוטומטיים
- **Views** לחישובים מורכבים
- **Indexes** לביצועים

### ORM - sqlite-net-pcl
```csharp
[Table("detection_events")]
public class DetectionEvent {
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }
    
    [Indexed]
    public DateTime Timestamp { get; set; }
}
```

---

## 🎤 Audio Processing Pipeline

```
Microphone
    ↓
[Voice Activity Detection]  ← Always running, low power
    ↓
[Picovoice Porcupine]      ← Wake word detection
    ↓
Keyword detected? 
    ↓ Yes
[Count & Track]
    ↓
3+ in 30 sec?
    ↓ Yes
[Alert User]
```

**Optimization:**
- VAD (Voice Activity Detection) קודם
- רק אז Picovoice המלא
- חוסך סוללה

---

## 📱 UI Framework

### XAML + Material Design
```xml
<ContentPage>
    <ScrollView>
        <VerticalStackLayout>
            <Frame>
                <Label />
            </Frame>
        </VerticalStackLayout>
    </ScrollView>
</ContentPage>
```

### RTL Support
```csharp
FlowDirection="RightToLeft"
```

### Theming
- Light/Dark modes
- Dynamic colors
- Accessibility support

---

## 🔐 Security & Privacy

### On-Device Processing
- ✅ כל זיהוי קול - במכשיר
- ✅ אין שליחה לשרת
- ✅ אין שמירת אודיו

### Data Encryption
```csharp
// SQLite encryption (future)
var options = new SQLiteConnectionString(
    databasePath,
    SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.Create,
    storeDateTimeAsTicks: true,
    key: encryptionKey  // AES-256
);
```

### Permissions
```xml
<!-- Android -->
<uses-permission android:name="android.permission.RECORD_AUDIO" />
<uses-permission android:name="android.permission.FOREGROUND_SERVICE" />

<!-- iOS -->
<key>NSMicrophoneUsageDescription</key>
<string>לזיהוי כשהילד קורא לך</string>
```

---

## 📊 Analytics & Monitoring

### Local Analytics
- SQLite `analytics_events` table
- אין שליחה external
- Privacy-first

### Crash Reporting (Future)
- אופציונלי
- Opt-in only
- Anonymous

---

## 🧪 Testing Strategy

### Unit Tests
```csharp
[Fact]
public async Task SaveDetectionEvent_Should_Save_Successfully()
{
    // Arrange
    var service = new DatabaseService(":memory:");
    var detection = new DetectionEvent { ... };
    
    // Act
    await service.SaveDetectionEventAsync(detection);
    
    // Assert
    Assert.NotEqual(0, detection.Id);
}
```

### Integration Tests
- Test עם SQLite אמיתי
- Test Picovoice integration
- Test permissions

### Manual Testing
- על מכשיר אמיתי
- עם ילדים אמיתיים! 😊

---

## 🚀 Build & Deployment

### Debug Build
```bash
dotnet build HereForYou.csproj -c Debug
```

### Release Build
```bash
dotnet publish HereForYou.csproj -c Release \
    -f net8.0-android \
    /p:AndroidPackageFormats=apk
```

### Distribution
- **Alpha:** Google Play Internal Testing
- **Beta:** Google Play Closed Beta
- **Release:** Google Play Store

---

## 📈 Performance Considerations

### Battery Optimization
- VAD לפני זיהוי מלא
- Sleep modes כשמסך כבוי
- Efficient SQLite queries

### Memory Management
- Dispose של services
- Image caching
- Lazy loading

### Startup Time
- Async initialization
- Splash screen
- Background loading

---

## 🔄 Future Tech Considerations

### Sync (Phase 6+)
- **SignalR** - real-time sync
- **Azure Functions** - serverless backend
- **Cosmos DB** - cloud storage

### AI Features (V2+)
- **ML.NET** - on-device ML
- **ONNX Runtime** - emotion detection
- **TensorFlow Lite** - custom models

---

## 📚 Resources

### Official Docs
- [.NET MAUI](https://learn.microsoft.com/dotnet/maui/)
- [SQLite](https://www.sqlite.org/docs.html)
- [Picovoice](https://picovoice.ai/docs/)

### Community
- [.NET MAUI GitHub](https://github.com/dotnet/maui)
- [Stack Overflow](https://stackoverflow.com/questions/tagged/.net-maui)
- [Discord Community](https://discord.gg/dotnet)

---

**Next: Read `05_DATABASE_DESIGN.md` for schema details 👉**
