# HereForYou - אפליקציית נוכחות הורית

**Version:** 0.5.0 - Near Production Ready
**Last Updated:** December 28, 2025
**Status:** ✅ Fully Functional with Mock Audio Detection

---

## 📋 תיאור הפרויקט

**HereForYou** היא אפליקציה חדשנית שעוזרת להורים להישאר קשובים לילדיהם גם כשהם עסוקים במסך הטלפון או המחשב. האפליקציה מזהה באופן אוטומטי כאשר הילד קורא "אמא" או "אבא" ומתריעה להורה בצורה מותאמת אישית.

### מטרת האפליקציה
- **ייצור נוכחות הורית** גם כאשר ההורה עסוק במסך
- **מעקב ותיעוד** של מספר הפניות של הילד
- **ניתוח סטטיסטי** של זמני מסך ונוכחות הורית
- **התראות מותאמות אישית** בהתאם לחומרת המצב

---

## ✨ פיצ'רים מושלמים

### 🎯 ניטור זיהוי מילות מפתח
- ✅ **MockAudioMonitorService** - סימולציה מלאה של זיהוי קול
  - זיהוי אוטומטי של "אמא" / "אבא" כל 10-30 שניות
  - רמות ביטחון (confidence) אקראיות
  - אינטגרציה מלאה עם מערכת ההתראות

### 🔔 מערכת התראות אמיתית
- ✅ **התראות סטנדרטיות** - DisplayAlert עם אפשרות לבחור "טופלתי" או "התעלמתי"
- ✅ **התראות קריטיות** - מסך אדום מלא עם כפתור "טופלתי בזה!"
- ✅ **Toast Notifications** - הודעות קלות למידע כללי
- ✅ **רטט** - תמיכה מלאה במכשירים שתומכים
- ✅ **צלילים** - מוכן לקבצי אודיו (כרגע placeholder)

### ⏱️ מעקב זמן מסך אמיתי
- ✅ **ScreenMonitorService** - מעקב אחר זמן השימוש באפליקציה
- ✅ **סשנים** - התחלה, עצירה, ועדכון כל דקה
- ✅ **סטטיסטיקות** - זמן מסך יומי, שבועי, וחודשי
- ✅ **אינטגרציה** - עדכון אוטומטי ב-UI

### 📊 ניתוח ותובנות
- ✅ **DailySummary** - סיכום יומי של זיהויים, התראות, וזמן מסך
- ✅ **WeeklyStats** - ממוצעים שבועיים ומגמות
- ✅ **MonthlyStats** - ניתוח חודשי מקיף
- ✅ **Response Rate** - אחוז התגובה להתראות

### ⚙️ הגדרות מתקדמות
- ✅ **סף זיהוי** - כמה פעמים צריך לקרוא עד התראה (1-10)
- ✅ **חלון זמן** - בתוך כמה שניות (10-300)
- ✅ **רמת ביטחון** - סף זיהוי מינימלי (0.5-0.95)
- ✅ **מילות מפתח** - ניתן להוסיף/להסיר מילים
- ✅ **התראות** - הפעלה/כיבוי של overlay, צלילים, רטט
- ✅ **שם משתמש** - התאמה אישית

### 🗄️ מסד נתונים SQLite
- ✅ **DetectionEvents** - כל זיהוי נשמר עם timestamp וביטחון
- ✅ **Alerts** - תיעוד כל התראה עם רמת חומרה
- ✅ **AlertResponses** - מעקב אחר תגובות המשתמש
- ✅ **ScreenTimeSessions** - כל סשן שימוש נשמר
- ✅ **DailySummaries** - סיכומים יומיים אוטומטיים
- ✅ **AnalyticsEvents** - לוג מלא של פעולות
- ✅ **UserSettings** - כל ההגדרות נשמרות

---

## 🏗️ ארכיטקטורה

```
┌─────────────────────────────────────────┐
│         MAUI Application (UI)           │
│  MainPage │ SettingsPage │ InsightsPage│
└──────────────────┬──────────────────────┘
                   │
┌──────────────────▼──────────────────────┐
│           ViewModels (MVVM)             │
│   MainVM │ SettingsVM │ InsightsVM      │
└──────────────────┬──────────────────────┘
                   │
┌──────────────────▼──────────────────────┐
│             Services Layer              │
│ ┌─────────────────────────────────────┐ │
│ │  AudioMonitor (Mock/Real)           │ │
│ │  NotificationService                │ │
│ │  ScreenMonitorService               │ │
│ │  AlertCoordinatorService            │ │
│ │  AnalyticsService                   │ │
│ │  SettingsService                    │ │
│ └─────────────────────────────────────┘ │
└──────────────────┬──────────────────────┘
                   │
┌──────────────────▼──────────────────────┐
│         DatabaseService (SQLite)        │
│           hereforyou.db3                │
└─────────────────────────────────────────┘
```

### טכנולוגיות
- **.NET MAUI 10.0** - פריימוורק חוצה פלטפורמות
- **SQLite** - מסד נתונים מקומי
- **CommunityToolkit.Mvvm** - MVVM קל ונקי
- **CommunityToolkit.Maui** - Toast notifications ועוד
- **Vosk 0.3.38** - מוכן לזיהוי קול אמיתי (אנגלית)

---

## 🚀 איך להריץ

### 📖 מדריכים מפורטים

- **[INSTALL.md](INSTALL.md)** - התקנה והרצה על Windows
- **[INSTALL_EMULATOR_STEP_BY_STEP.md](INSTALL_EMULATOR_STEP_BY_STEP.md)** - התקנת Android Emulator (מומלץ!)
- **[ANDROID_SETUP.md](ANDROID_SETUP.md)** - אפשרויות פריסה ל-Android
- **[QUICK_ANDROID_GUIDE.md](QUICK_ANDROID_GUIDE.md)** - מדריך מהיר לAndroid

### הרצה מהירה על Windows

```bash
# Clone the repository
git clone https://github.com/dorongut1/HereForYou.git
cd HereForYou

# הרץ מקוד מקור
dotnet run -f net10.0-windows10.0.19041.0

# או הרץ את ה-EXE המוכן:
.\bin\Release\net10.0-windows10.0.19041.0\win-x64\publish\HereForYou.exe
```

### הרצה על Android Emulator

```bash
# 1. התקן Android Studio והגדר emulator (ראה INSTALL_EMULATOR_STEP_BY_STEP.md)
# 2. הפעל את ה-emulator
# 3. הרץ:

dotnet build -f net10.0-android
dotnet run -f net10.0-android
```

### או ב-Visual Studio 2022
1. פתח את `HereForYou.sln`
2. בחר Target: **Windows Machine** או **Android Emulator**
3. לחץ **F5** להרצה

---

## 📱 איך זה עובד

### תרחיש שימוש טיפוסי

1. **הפעלת ניטור**
   ```
   אתה: לוחץ על "התחל ניטור"
   האפליקציה: מתחילה להאזין ולעקוב אחר זמן מסך
   ```

2. **זיהוי אירוע**
   ```
   MockAudio: מדמה זיהוי של "אמא" (כל 10-30 שניות)
   Database: שומר את האירוע + timestamp + confidence
   AlertCoordinator: בודק אם הגענו לסף (ברירת מחדל: 3 פעמים)
   ```

3. **התראה**
   ```
   (אם הגענו לסף)
   NotificationService: מציג התראה לפי רמת חומרה
   - Info: Toast notification
   - Warning: DisplayAlert רגיל
   - Critical: מסך אדום מלא + רטט
   ```

4. **תגובה**
   ```
   אתה: לוחץ "טופלתי בזה" או "התעלמתי"
   Database: שומר את התגובה + זמן תגובה
   Analytics: מעדכן סטטיסטיקות
   ```

5. **תובנות**
   ```
   InsightsPage: מציג:
   - כמה פעמים קרא לך הילד היום
   - כמה זמן היית במסך
   - מה אחוז התגובה שלך
   - מגמות לאורך זמן
   ```

---

## 📊 מה עובד VS מה לא

### ✅ מושלם ועובד

| קומפוננטה | סטטוס | הערות |
|-----------|-------|-------|
| UI + ניווט | ✅ | 3 עמודים עם TabBar, RTL בעברית |
| Database | ✅ | SQLite מלא עם 7 טבלאות |
| MockAudioMonitor | ✅ | סימולציה מושלמת של זיהוי קול |
| NotificationService | ✅ | Alerts, Toasts, Overlays, רטט |
| ScreenMonitor | ✅ | מעקב זמן מסך אמיתי |
| AlertCoordinator | ✅ | לוגיקת סף ורמות חומרה |
| Analytics | ✅ | חישובי סטטיסטיקות מתקדמים |
| Settings | ✅ | שמירה וטעינה של כל ההגדרות |
| ViewModels | ✅ | MVVM נקי עם data binding |

### ⏳ בתהליך / לעתיד

| קומפוננטה | סטטוס | הערות |
|-----------|-------|-------|
| זיהוי קול **אמיתי** | ⏳ | Vosk מותקן, צריך מודל אנגלית |
| תמיכה בעברית | ⏳ | אין מודל זיהוי קול בעברית ב-Vosk |
| צלילי התראה | ⏳ | המבנה קיים, צריך קבצי MP3 |
| Android/iOS | ⏳ | הכל מוכן, צריך build ובדיקות |
| App Icon | ⏳ | אופציונלי |
| הרשאות | ⏳ | יידרש רק ב-Android/iOS |

---

## 🎯 מה הלאה

### שלב הבא (Phase 10): זיהוי קול אמיתי
```csharp
// 1. הורד מודל Vosk אנגלית
// 2. צור RealAudioMonitorService
// 3. החלף את MockAudioMonitorService ב-DI
// 4. הוסף microphone permissions
// 5. בדוק על מכשיר אמיתי
```

### עתיד רחוק יותר
- **Cloud Sync** - גיבוי ענן של הנתונים
- **Multi-User** - תמיכה במספר ילדים
- **Advanced Analytics** - תרשימים וגרפים
- **Smart Notifications** - למידת מכונה לשיפור ההתראות
- **Hebrew Speech Recognition** - אימון מודל עברית מותאם אישית

---

## 📝 בעיות ידועות

### Warnings (לא בלוקינג)
- **MVVMTK0045**: AOT compatibility warnings - לא משפיע על Windows
- **CS0618**: Application.MainPage deprecated - עובד מצוין, רק deprecated
- **CS0067**: Unused events - יהיו בשימוש בעתיד

### Limitations
- **רק Windows בינתיים** - Android/iOS מוכנים אבל לא נבדקו
- **Mock Audio** - לא זיהוי קול אמיתי (עדיין!)
- **אין צלילים** - צריך להוסיף קבצי MP3
- **אין עברית** - Vosk לא תומך בעברית

---

## 🤝 תרומה לפרויקט

רוצה לעזור? הנה איפה צריך עזרה:
1. **אימון מודל עברית** לזיהוי קול
2. **עיצוב App Icon** מקצועי
3. **קבצי צלילים** להתראות
4. **תרגום** לשפות נוספות
5. **בדיקות** על Android/iOS

---

## 📄 רישיון

MIT License - השתמש בחופשיות!

---

## 🙏 תודות

- **Vosk** - זיהוי קול offline מעולה
- **CommunityToolkit** - כלים נהדרים ל-MAUI
- **SQLite** - מסד נתונים קל ומהיר
- **Claude** - עזרה בכתיבת הקוד 🤖

---

**נבנה באהבה עבור הורים שרוצים להיות יותר נוכחים ♥️**

**Last Build:** December 28, 2025
**Commits:** 7
**Lines of Code:** ~3,500
**Coffee Consumed:** ☕☕☕
