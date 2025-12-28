# הרצת HereForYou על Android

## אפשרות 1: אמולטור Android על Windows (מומלץ)

### שלב 1: התקנת Android Emulator

#### בחירה א': Visual Studio Android Emulator
1. **פתח Visual Studio Installer**
2. לחץ "Modify" על Visual Studio 2022
3. בחר את הטאב "Individual components"
4. סמן:
   - ✅ Android SDK Platform (API 34)
   - ✅ Android SDK Build-Tools
   - ✅ Android Emulator
5. לחץ "Install"

#### בחירה ב': Android Studio (יותר חזק)
1. הורד מ: https://developer.android.com/studio
2. התקן Android Studio
3. פתח Android Studio
4. עבור ל: Tools → Device Manager
5. לחץ "Create Device"
6. בחר: Pixel 7 (או כל מכשיר אחר)
7. בחר: System Image - API 34 (Android 14)
8. לחץ "Finish"

---

### שלב 2: הרצת האמולטור

#### אם השתמשת ב-Visual Studio:
```bash
# פתח Command Prompt ב-C:\Disk1\HereForYou
cd C:\Disk1\HereForYou

# בדוק שהאמולטור זמין
"%LOCALAPPDATA%\Android\Sdk\emulator\emulator" -list-avds

# הרץ את האמולטור (החלף AVD_NAME בשם שקיבלת)
"%LOCALAPPDATA%\Android\Sdk\emulator\emulator" -avd [AVD_NAME]
```

#### אם השתמשת ב-Android Studio:
1. פתח Device Manager
2. לחץ ▶️ ליד המכשיר שיצרת
3. האמולטור יפתח

---

### שלב 3: הרצת האפליקציה על האמולטור

```bash
# ודא שהאמולטור פועל
adb devices

# אמור להראות משהו כמו:
# List of devices attached
# emulator-5554   device

# Build והתקנה על האמולטור
dotnet build -f net10.0-android -c Debug
dotnet run -f net10.0-android --no-build
```

---

## אפשרות 2: Build לטלפון Android אמיתי

### דרישות
- טלפון Android עם USB Debugging
- כבל USB

### שלב 1: הפעלת Developer Mode בטלפון

1. **הגדרות → About Phone**
2. לחץ על "Build Number" **7 פעמים**
3. יופיע: "You are now a developer!"

### שלב 2: הפעלת USB Debugging

1. **הגדרות → Developer Options**
2. הפעל "USB Debugging"
3. חבר את הטלפון למחשב עם USB

### שלב 3: אישור החיבור

```bash
# בדוק שהטלפון מחובר
adb devices

# אם מופיע "unauthorized" - אשר את החיבור בטלפון
```

### שלב 4: Build והתקנה

```bash
cd C:\Disk1\HereForYou

# Build Release APK
dotnet publish -f net10.0-android -c Release

# התקנה בטלפון
adb install "bin\Release\net10.0-android\publish\com.hereforyou.app-Signed.apk"
```

---

## אפשרות 3: Windows Subsystem for Android (WSA)

### התקנת WSA (Windows 11 בלבד)

1. פתח Microsoft Store
2. חפש "Amazon Appstore"
3. התקן (זה יתקין גם את WSA)
4. הפעל את "Windows Subsystem for Android Settings"
5. הפעל "Developer Mode"

### Build והרצה על WSA

```bash
cd C:\Disk1\HereForYou

# Build
dotnet build -f net10.0-android -c Release

# התקנה ב-WSA
adb connect 127.0.0.1:58526
adb install "bin\Release\net10.0-android\publish\com.hereforyou.app-Signed.apk"
```

---

## בעיות נפוצות

### שגיאה: "adb is not recognized"

הוסף את Android SDK ל-PATH:
```
%LOCALAPPDATA%\Android\Sdk\platform-tools
```

### שגיאה: "No emulators found"

```bash
# התקן Android Emulator דרך SDK Manager
sdkmanager "system-images;android-34;google_apis;x86_64"
sdkmanager "emulator"
```

### שגיאה: "Build failed for Android"

וודא שיש לך:
```bash
# בדוק Android SDK
dotnet workload list

# אם לא רשום "android" - התקן:
dotnet workload install android
```

---

## הרצה מהירה (TL;DR)

### עם Android Studio:
```bash
# 1. התקן Android Studio
# 2. צור AVD (Virtual Device)
# 3. הרץ את האמולטור
# 4. בנה והרץ:

cd C:\Disk1\HereForYou
dotnet build -f net10.0-android
dotnet run -f net10.0-android
```

### עם טלפון אמיתי:
```bash
# 1. הפעל USB Debugging בטלפון
# 2. חבר USB
# 3. אשר את החיבור
# 4. בנה והתקן:

cd C:\Disk1\HereForYou
adb devices
dotnet publish -f net10.0-android -c Release
adb install "bin\Release\net10.0-android\publish\*.apk"
```

---

## הערות חשובות

### הרשאות Android
האפליקציה תדרוש הרשאות:
- **Microphone** - לזיהוי קול (Phase 10)
- **Vibrate** - לרטט בהתראות
- **Display over other apps** - ל-overlay alerts (אופציונלי)

### ביצועים
- **אמולטור**: איטי יותר, טוב לבדיקות
- **טלפון אמיתי**: מהיר, חווית משתמש אמיתית
- **WSA**: טוב ל-Windows 11, חווית hybrid

### Debugging
להצגת לוגים בזמן אמת:
```bash
adb logcat | findstr "HereForYou"
```

---

**אם תקלת בבעיה - צרו Issue ב-GitHub!**
https://github.com/dorongut1/HereForYou/issues
