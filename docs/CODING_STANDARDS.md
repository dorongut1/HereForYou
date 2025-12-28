# HereForYou - Coding Standards & Conventions
## מסמך סטנדרטים לפיתוח

**Version:** 1.0
**Last Updated:** 2024-12-27

---

## 1. Project Structure

```
HereForYou/
├── src/
│   └── HereForYou/                    # Main MAUI App
│       ├── App.xaml                   # Application entry
│       ├── MauiProgram.cs             # DI Configuration
│       ├── Models/                    # Entity classes
│       ├── Services/                  # Business logic
│       │   ├── Interfaces/            # Service contracts
│       │   └── Implementations/       # Service implementations
│       ├── ViewModels/                # MVVM ViewModels
│       ├── Views/                     # XAML Pages
│       ├── Converters/                # Value converters
│       ├── Helpers/                   # Utility classes
│       ├── Resources/                 # Styles, fonts, images
│       └── Platforms/                 # Platform-specific code
│           ├── Android/
│           ├── iOS/
│           └── Windows/
├── tests/
│   └── HereForYou.Tests/              # Unit tests
├── docs/                              # Documentation
└── Database/                          # SQL scripts
```

---

## 2. Naming Conventions

### 2.1 General Rules

| Element | Convention | Example |
|---------|------------|---------|
| Namespace | PascalCase | `HereForYou.Services` |
| Class | PascalCase | `DatabaseService` |
| Interface | I + PascalCase | `IDatabaseService` |
| Method | PascalCase | `GetDetectionEventsAsync` |
| Property | PascalCase | `IsMonitoring` |
| Private field | _camelCase | `_database` |
| Parameter | camelCase | `detectionEvent` |
| Local variable | camelCase | `totalCount` |
| Constant | PascalCase | `MaxRetryCount` |
| Async method | Suffix with Async | `SaveAsync` |

### 2.2 File Naming

| Type | Pattern | Example |
|------|---------|---------|
| Model | `{Name}.cs` | `DetectionEvent.cs` |
| Service Interface | `I{Name}Service.cs` | `IDatabaseService.cs` |
| Service Implementation | `{Name}Service.cs` | `DatabaseService.cs` |
| ViewModel | `{Name}ViewModel.cs` | `MainViewModel.cs` |
| View | `{Name}Page.xaml` | `MainPage.xaml` |
| Converter | `{Name}Converter.cs` | `BoolToColorConverter.cs` |

---

## 3. Code Style

### 3.1 Using Statements

```csharp
// System namespaces first
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

// Third-party namespaces
using CommunityToolkit.Mvvm.ComponentModel;
using SQLite;

// Project namespaces
using HereForYou.Models;
using HereForYou.Services.Interfaces;
```

### 3.2 Class Structure Order

```csharp
public class ExampleService : IExampleService
{
    // 1. Constants
    private const int MaxRetries = 3;

    // 2. Private fields
    private readonly IDatabaseService _database;
    private bool _isInitialized;

    // 3. Constructor
    public ExampleService(IDatabaseService database)
    {
        _database = database;
    }

    // 4. Public properties
    public bool IsReady => _isInitialized;

    // 5. Events
    public event EventHandler<EventArgs> DataChanged;

    // 6. Public methods
    public async Task InitializeAsync()
    {
        // Implementation
    }

    // 7. Private methods
    private void OnDataChanged()
    {
        DataChanged?.Invoke(this, EventArgs.Empty);
    }
}
```

### 3.3 Braces & Indentation

```csharp
// Always use braces, even for single-line
if (condition)
{
    DoSomething();
}

// Use 4 spaces for indentation (not tabs)
public void Method()
{
    if (condition)
    {
        // Nested code
    }
}
```

### 3.4 Async/Await

```csharp
// Always use Async suffix
public async Task<List<DetectionEvent>> GetEventsAsync()
{
    return await _database.Table<DetectionEvent>().ToListAsync();
}

// Use ConfigureAwait(false) in library code
public async Task SaveAsync(DetectionEvent item)
{
    await _database.InsertAsync(item).ConfigureAwait(false);
}

// Never use .Result or .Wait() - always await
// BAD: var result = GetDataAsync().Result;
// GOOD: var result = await GetDataAsync();
```

---

## 4. MVVM Guidelines

### 4.1 ViewModel Pattern

```csharp
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

public partial class MainViewModel : ObservableObject
{
    private readonly IDatabaseService _database;

    // Use [ObservableProperty] for bindable properties
    [ObservableProperty]
    private bool _isMonitoring;

    [ObservableProperty]
    private string _statusMessage = "Ready";

    // Use [RelayCommand] for commands
    [RelayCommand]
    private async Task StartMonitoringAsync()
    {
        IsMonitoring = true;
        StatusMessage = "Monitoring...";
    }

    // Constructor injection
    public MainViewModel(IDatabaseService database)
    {
        _database = database;
    }
}
```

### 4.2 View (XAML) Pattern

```xml
<?xml version="1.0" encoding="utf-8" ?>
<ContentPage xmlns="http://schemas.microsoft.com/dotnet/2021/maui"
             xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml"
             xmlns:vm="clr-namespace:HereForYou.ViewModels"
             x:Class="HereForYou.Views.MainPage"
             x:DataType="vm:MainViewModel"
             Title="Main">

    <!-- Always use x:DataType for compile-time binding -->
    <VerticalStackLayout>
        <Label Text="{Binding StatusMessage}" />
        <Button Text="Start" Command="{Binding StartMonitoringCommand}" />
    </VerticalStackLayout>

</ContentPage>
```

---

## 5. Dependency Injection

### 5.1 Registration in MauiProgram.cs

```csharp
public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();

        // Services - Singleton for shared state
        builder.Services.AddSingleton<IDatabaseService, DatabaseService>();
        builder.Services.AddSingleton<IAudioMonitorService, AudioMonitorService>();

        // ViewModels - Transient for fresh instances
        builder.Services.AddTransient<MainViewModel>();
        builder.Services.AddTransient<InsightsViewModel>();

        // Views - Transient
        builder.Services.AddTransient<MainPage>();
        builder.Services.AddTransient<InsightsPage>();

        return builder.Build();
    }
}
```

### 5.2 Constructor Injection

```csharp
// Always use constructor injection
public class MyService : IMyService
{
    private readonly IDatabaseService _database;
    private readonly INotificationService _notifications;

    public MyService(
        IDatabaseService database,
        INotificationService notifications)
    {
        _database = database ?? throw new ArgumentNullException(nameof(database));
        _notifications = notifications ?? throw new ArgumentNullException(nameof(notifications));
    }
}
```

---

## 6. Error Handling

### 6.1 Exception Handling Pattern

```csharp
public async Task<Result<T>> SafeExecuteAsync<T>(Func<Task<T>> action)
{
    try
    {
        var result = await action();
        return Result<T>.Success(result);
    }
    catch (SQLiteException ex)
    {
        // Log and return failure
        Debug.WriteLine($"Database error: {ex.Message}");
        return Result<T>.Failure($"Database error: {ex.Message}");
    }
    catch (Exception ex)
    {
        Debug.WriteLine($"Unexpected error: {ex.Message}");
        return Result<T>.Failure($"Unexpected error: {ex.Message}");
    }
}
```

### 6.2 Result Pattern

```csharp
public class Result<T>
{
    public bool IsSuccess { get; private set; }
    public T Value { get; private set; }
    public string Error { get; private set; }

    public static Result<T> Success(T value) => new() { IsSuccess = true, Value = value };
    public static Result<T> Failure(string error) => new() { IsSuccess = false, Error = error };
}
```

---

## 7. Database Access

### 7.1 Repository Pattern

```csharp
public interface IRepository<T> where T : class, new()
{
    Task<T> GetByIdAsync(int id);
    Task<List<T>> GetAllAsync();
    Task<int> SaveAsync(T entity);
    Task<int> DeleteAsync(int id);
}
```

### 7.2 Query Patterns

```csharp
// Use async methods
public async Task<List<DetectionEvent>> GetRecentEventsAsync(int count)
{
    return await _database.Table<DetectionEvent>()
        .OrderByDescending(e => e.Timestamp)
        .Take(count)
        .ToListAsync();
}

// Use parameterized queries for safety
public async Task<List<DetectionEvent>> GetByKeywordAsync(string keyword)
{
    return await _database.Table<DetectionEvent>()
        .Where(e => e.Keyword == keyword)
        .ToListAsync();
}
```

---

## 8. Testing Standards

### 8.1 Test Naming

```csharp
// Pattern: MethodName_Scenario_ExpectedResult
[Fact]
public async Task GetByIdAsync_ValidId_ReturnsEntity()
{
    // Arrange
    // Act
    // Assert
}

[Fact]
public async Task SaveAsync_NewEntity_ReturnsPositiveId()
{
    // ...
}

[Fact]
public async Task DeleteAsync_NonExistentId_ReturnsZero()
{
    // ...
}
```

### 8.2 Test Structure (AAA)

```csharp
[Fact]
public async Task SaveDetectionEvent_ValidEvent_SavesSuccessfully()
{
    // Arrange
    var service = new DatabaseService(":memory:");
    await service.InitializeAsync();
    var detection = new DetectionEvent
    {
        Keyword = "אמא",
        Timestamp = DateTime.Now,
        Confidence = 0.9f
    };

    // Act
    var id = await service.SaveDetectionEventAsync(detection);

    // Assert
    Assert.True(id > 0);
    var saved = await service.GetDetectionEventByIdAsync(id);
    Assert.NotNull(saved);
    Assert.Equal("אמא", saved.Keyword);
}
```

---

## 9. Documentation

### 9.1 XML Comments for Public APIs

```csharp
/// <summary>
/// Saves a detection event to the database.
/// </summary>
/// <param name="detectionEvent">The detection event to save.</param>
/// <returns>The ID of the saved event.</returns>
/// <exception cref="ArgumentNullException">Thrown when detectionEvent is null.</exception>
public async Task<int> SaveDetectionEventAsync(DetectionEvent detectionEvent)
{
    if (detectionEvent == null)
        throw new ArgumentNullException(nameof(detectionEvent));

    return await _database.InsertAsync(detectionEvent);
}
```

### 9.2 Inline Comments

```csharp
// Use comments for "why", not "what"

// BAD: Increment counter
count++;

// GOOD: Track consecutive detections for alert threshold
consecutiveDetections++;
```

---

## 10. Platform-Specific Code

### 10.1 Conditional Compilation

```csharp
#if ANDROID
    // Android-specific code
    builder.Services.AddSingleton<IAudioMonitorService, AndroidAudioMonitorService>();
#elif IOS
    // iOS-specific code
    builder.Services.AddSingleton<IAudioMonitorService, IOSAudioMonitorService>();
#elif WINDOWS
    // Windows-specific code
    builder.Services.AddSingleton<IAudioMonitorService, WindowsAudioMonitorService>();
#endif
```

### 10.2 Partial Classes for Platform Code

```csharp
// Shared code: Services/AudioMonitorService.cs
public partial class AudioMonitorService : IAudioMonitorService
{
    public partial Task<bool> StartMonitoringAsync();
    public partial Task StopMonitoringAsync();
}

// Android: Platforms/Android/Services/AudioMonitorService.cs
public partial class AudioMonitorService
{
    public partial async Task<bool> StartMonitoringAsync()
    {
        // Android implementation
    }
}
```

---

## 11. Security & Privacy

### 11.1 Sensitive Data

```csharp
// Never log sensitive data
Debug.WriteLine($"User logged in"); // GOOD
Debug.WriteLine($"User {email} with password {pwd}"); // BAD

// Use SecureStorage for sensitive data
await SecureStorage.SetAsync("api_key", apiKey);
var apiKey = await SecureStorage.GetAsync("api_key");
```

### 11.2 Permissions

```csharp
// Always check permissions before use
public async Task<bool> CheckMicrophonePermissionAsync()
{
    var status = await Permissions.CheckStatusAsync<Permissions.Microphone>();

    if (status != PermissionStatus.Granted)
    {
        status = await Permissions.RequestAsync<Permissions.Microphone>();
    }

    return status == PermissionStatus.Granted;
}
```

---

## 12. StyleCop Rules (Enforced)

### Enabled Rules

```xml
<!-- .editorconfig -->
# StyleCop Analyzers
dotnet_diagnostic.SA1000.severity = warning  # Keywords should be spaced correctly
dotnet_diagnostic.SA1001.severity = warning  # Commas should be spaced correctly
dotnet_diagnostic.SA1025.severity = warning  # Code should not contain multiple whitespace
dotnet_diagnostic.SA1028.severity = warning  # Code should not contain trailing whitespace
dotnet_diagnostic.SA1101.severity = none     # Prefix local calls with this (disabled)
dotnet_diagnostic.SA1200.severity = warning  # Using directives should be placed correctly
dotnet_diagnostic.SA1309.severity = none     # Field names should not begin with underscore (disabled - we use _)
dotnet_diagnostic.SA1633.severity = none     # File should have header (disabled)
```

---

## 13. Git Commit Messages

### Format

```
<type>: <subject>

<body>

🤖 Generated with [Claude Code](https://claude.com/claude-code)

Co-Authored-By: Claude Opus 4.5 <noreply@anthropic.com>
```

### Types

| Type | Description |
|------|-------------|
| feat | New feature |
| fix | Bug fix |
| docs | Documentation |
| style | Formatting, no code change |
| refactor | Code restructuring |
| test | Adding tests |
| chore | Maintenance |

### Example

```
feat: Add voice detection service with Picovoice integration

- Implement AudioMonitorService with Porcupine SDK
- Add support for Hebrew keywords (אמא, אבא)
- Include permission handling for microphone

🤖 Generated with [Claude Code](https://claude.com/claude-code)

Co-Authored-By: Claude Opus 4.5 <noreply@anthropic.com>
```

---

## 14. Build & Quality Gates

### Before Each Commit

1. ✅ Code compiles without errors
2. ✅ No StyleCop warnings (critical)
3. ✅ All unit tests pass
4. ✅ No TODO comments without issue reference

### CI/CD Checks

```bash
# Build
dotnet build --configuration Release

# Run tests
dotnet test --no-build --configuration Release

# Check for warnings as errors
dotnet build --configuration Release /warnaserror
```

---

**This document is the source of truth for all code written in this project.**
