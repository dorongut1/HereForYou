# Test Plan - HereForYou Application

## Overview
This document outlines the testing strategy for the HereForYou parental presence application.

## Test Categories

### 1. Database Tests
**File:** `Services/DatabaseService.cs`

#### Test Cases:
- [x] Database initialization creates all tables
- [ ] Schema version tracking works correctly
- [ ] DetectionEvents can be created, read, updated, deleted
- [ ] DailySummaries can be created and queried
- [ ] AlertResponses can be created and retrieved
- [ ] UserSettings can be saved and loaded
- [ ] Date range queries work correctly
- [ ] GetTotalScreenTimeAsync calculates correctly
- [ ] Concurrent access doesn't corrupt data

### 2. Settings Service Tests
**File:** `Services/SettingsService.cs`

#### Test Cases:
- [ ] Default settings load correctly on first run
- [ ] Settings persist across app restarts
- [ ] All setting types (bool, int, float, string) work
- [ ] Invalid values are handled gracefully
- [ ] Concurrent setting updates work

### 3. Audio Monitor Service Tests (Mock)
**File:** `Services/MockAudioMonitorService.cs`

#### Test Cases:
- [x] Service starts and stops monitoring
- [x] Mock detections fire periodically (10-30s)
- [x] KeywordDetected event includes correct data
- [x] MonitoringStateChanged event fires on state change
- [ ] Multiple start calls are idempotent
- [ ] Stop cancels detection loop
- [ ] Active keywords list is read-only

### 4. Notification Service Tests
**File:** `Services/NotificationService.cs`

#### Test Cases:
- [x] All notification methods execute without errors
- [x] Debug output appears in logs
- [ ] Permission methods return expected values
- [ ] Overlay methods can be called safely
- [ ] Sound and vibration methods work

### 5. Alert Coordinator Tests
**File:** `Services/AlertCoordinatorService.cs`

#### Test Cases:
- [ ] Detection events are tracked correctly
- [ ] Threshold logic triggers alerts appropriately
- [ ] Response tracking updates database
- [ ] Different alert levels trigger correct actions
- [ ] Cooldown period prevents alert spam

### 6. MainViewModel Tests
**File:** `ViewModels/MainViewModel.cs`

#### Test Cases:
- [ ] InitializeAsync loads today's stats
- [ ] ToggleMonitoringAsync changes state
- [ ] Status message updates correctly
- [ ] Today's detections count updates
- [ ] Screen time displays in HH:MM format
- [ ] Properties notify UI of changes

### 7. SettingsViewModel Tests
**File:** `ViewModels/SettingsViewModel.cs`

#### Test Cases:
- [ ] LoadSettingsAsync populates all fields
- [ ] SaveSettingsAsync persists all changes
- [ ] TestNotificationCommand shows notification
- [ ] TestAlertCommand triggers alert
- [ ] Clear data confirms before deletion
- [ ] Two-way binding works for all properties

### 8. InsightsViewModel Tests
**File:** `ViewModels/InsightsViewModel.cs`

#### Test Cases:
- [ ] LoadStatsAsync calculates totals correctly
- [ ] Recent summaries are sorted by date
- [ ] Recent detections show latest first
- [ ] Response rate calculates correctly
- [ ] Screen time aggregation works
- [ ] Refresh updates all data

### 9. UI Navigation Tests
**File:** `AppShell.xaml`

#### Test Cases:
- [ ] TabBar shows all three tabs
- [ ] Navigation between tabs works
- [ ] RTL layout applies correctly
- [ ] Icons display properly
- [ ] Tab titles in Hebrew are correct

### 10. Data Binding Tests

#### MainPage:
- [ ] IsMonitoring toggles button state
- [ ] StatusMessage updates display
- [ ] TodayScreenTime formats correctly
- [ ] TodayDetections shows count
- [ ] Monitoring button changes color

#### SettingsPage:
- [ ] All sliders bind correctly
- [ ] Switches toggle settings
- [ ] Entry fields update on blur
- [ ] Save button saves all changes
- [ ] Test buttons trigger actions

#### InsightsPage:
- [ ] Statistics cards show data
- [ ] Recent summaries list populates
- [ ] Recent detections list shows events
- [ ] Refresh button reloads data
- [ ] Empty states show when no data

## Integration Tests

### End-to-End Flow 1: First Time User
1. Launch app for first time
2. Verify default settings loaded
3. Navigate to Settings tab
4. Verify all default values present
5. Return to Main tab
6. Start monitoring
7. Wait for mock detection (10-30s)
8. Verify detection appears in Insights
9. Stop monitoring
10. Verify monitoring stopped

### End-to-End Flow 2: Settings Persistence
1. Navigate to Settings
2. Change detection threshold to 5
3. Change confidence to 0.8
4. Save settings
5. Close app
6. Relaunch app
7. Navigate to Settings
8. Verify threshold is 5
9. Verify confidence is 0.8

### End-to-End Flow 3: Mock Detection Flow
1. Start monitoring
2. Wait for detection event
3. Verify event logged to database
4. Check Insights page
5. Verify detection appears in recent list
6. Verify today's count incremented
7. Continue monitoring for threshold
8. Verify alert coordinator triggered
9. Check alert response in database

## Performance Tests

### Database Performance:
- [ ] 1000 detections insert in <1s
- [ ] Query 30 days of data in <500ms
- [ ] Database file size stays reasonable (<10MB for 1 month)

### UI Performance:
- [ ] Page navigation <100ms
- [ ] List scrolling is smooth (60fps)
- [ ] Data binding updates <50ms

## Platform-Specific Tests

### Windows:
- [ ] App launches successfully
- [ ] All pages render correctly
- [ ] Database path is correct
- [ ] Notifications work
- [ ] App can run in background

### Android (Future):
- [ ] App requests necessary permissions
- [ ] Overlay permission handled
- [ ] Notification permission handled
- [ ] Background service works
- [ ] Battery optimization handled

## Regression Tests
After each code change:
1. Build succeeds with 0 errors
2. All three pages navigate
3. Monitoring can start/stop
4. Settings can be saved/loaded
5. Database operations work

## Test Status Summary
- Total Test Cases: 71
- Implemented: 8
- Passing: 8
- Failing: 0
- Not Yet Implemented: 63

## Notes
- Mock services allow testing without platform-specific dependencies
- Integration tests should be run on each target platform
- Performance benchmarks are targets, not strict requirements
- UI tests require manual verification until automated UI testing is set up
