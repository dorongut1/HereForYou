# מדריך צעד-אחר-צעד: התקנת Android Emulator

## 🎯 המלצה: Android Studio Emulator

זה האמולטור הכי טוב - מהיר, יציב, וקל להתקנה.

---

## שלב 1: הורדה והתקנה של Android Studio

### 1.1 הורד את Android Studio

1. גלוש ל: **https://developer.android.com/studio**
2. לחץ על **"Download Android Studio"**
3. קבל את התנאים ולחץ **"Download Android Studio for Windows"**
4. יורד קובץ: `android-studio-2024.3.1.12-windows.exe` (או דומה)

### 1.2 התקן

1. **הפעל את הקובץ שהורדת** (לחץ כפול)
2. **Next** → **Next** → **Install**
3. ⏳ זה ייקח כ-5-10 דקות
4. כשיסיים: **Next** → **Finish**
5. **תסמן** "Start Android Studio" ולחץ **Finish**

---

## שלב 2: Setup ראשוני של Android Studio

### 2.1 Welcome Screen

1. Android Studio ייפתח עם "Welcome" screen
2. **אל תפתח פרויקט!**
3. לחץ על **"More Actions"** (שלוש נקודות למטה)
4. בחר **"SDK Manager"**

### 2.2 התקנת Android SDK

בחלון **SDK Manager**:

1. **טאב "SDK Platforms":**
   - ✅ סמן: **Android 14.0 ("UpsideDownCake") - API Level 34**
   - לחץ **Apply**

2. **טאב "SDK Tools":**
   - ✅ סמן: **Android SDK Build-Tools 34**
   - ✅ סמן: **Android Emulator**
   - ✅ סמן: **Android SDK Platform-Tools**
   - לחץ **Apply**

3. יופיע חלון אישור - לחץ **OK**
4. ⏳ זה ייקח כ-5-10 דקות
5. כשיסיים - לחץ **Finish**
6. סגור את SDK Manager

---

## שלב 3: יצירת Android Virtual Device (AVD)

### 3.1 פתח את Device Manager

1. בחלון ה-Welcome של Android Studio
2. לחץ על **"More Actions"** (שלוש נקודות)
3. בחר **"Virtual Device Manager"**

### 3.2 צור מכשיר וירטואלי

1. לחץ **"Create Device"** (כפתור + גדול)

2. **בחר Hardware:**
   - קטגוריה: **Phone**
   - מכשיר: **Pixel 7** (מומלץ) או **Pixel 5**
   - לחץ **Next**

3. **בחר System Image:**
   - טאב: **Recommended**
   - בחר: **UpsideDownCake** (API Level 34, x86_64)
   - אם כתוב "Download" - לחץ עליו ו⏳ חכה להורדה
   - לחץ **Next**

4. **Verify Configuration:**
   - AVD Name: `Pixel_7_API_34` (אפשר לשנות)
   - Startup orientation: Portrait
   - **אל תשנה שום דבר אחר!**
   - לחץ **Finish**

5. ✅ האמולטור נוצר!

---

## שלב 4: הרצת האמולטור

### 4.1 הפעל את האמולטור

1. ב-**Device Manager**, תראה את המכשיר שיצרת
2. לחץ על **▶️** (Play) ליד שם המכשיר
3. ⏳ האמולטור יפתח (לוקח 30-60 שניות בפעם הראשונה)
4. ✅ תראה מסך טלפון Android!

### 4.2 וודא שהוא עובד

פתח **PowerShell** או **Command Prompt** והרץ:

```powershell
adb devices
```

אמור להראות:
```
List of devices attached
emulator-5554   device
```

✅ **מעולה! האמולטור פועל!**

---

## שלב 5: הרצת HereForYou על האמולטור

### 5.1 ודא שהאמולטור פועל

וודא שהחלון של האמולטור פתוח וב-PowerShell רואים:
```powershell
adb devices
# אמור להראות: emulator-5554   device
```

### 5.2 Build והרץ את האפליקציה

```powershell
# נווט לתיקיית הפרויקט
cd C:\Disk1\HereForYou

# Build עבור Android
dotnet build -f net10.0-android -c Debug

# הרץ על האמולטור
dotnet run -f net10.0-android
```

⏳ זה ייקח 1-2 דקות בפעם הראשונה

✅ **האפליקציה תיפתח באמולטור!**

---

## 🎉 סיימת!

עכשיו תוכל:
- ✅ לראות את האפליקציה פועלת
- ✅ ללחוץ על כפתורים
- ✅ לקבל התראות
- ✅ לבדוק את כל התכונות

---

## 💡 טיפים שימושיים

### סגירת האמולטור
- פשוט סגור את החלון של האמולטור (X)

### הרצה מחדש
```powershell
# הפעל את האמולטור (מ-Android Studio או:)
emulator -avd Pixel_7_API_34

# המתן עד שהוא מוכן
adb devices

# הרץ את האפליקציה
cd C:\Disk1\HereForYou
dotnet run -f net10.0-android
```

### Debug Logs
```powershell
# ראה לוגים של האפליקציה
adb logcat | findstr "HereForYou"
```

### מחיקת האפליקציה מהאמולטור
```powershell
adb uninstall com.companyname.hereforyou
```

### רשימת כל ה-AVDs שלך
```powershell
emulator -list-avds
```

---

## 🐛 בעיות נפוצות

### האמולטור לא נפתח
- וודא ש-Virtualization מופעל ב-BIOS
- נסה לסגור ולפתוח מחדש Android Studio

### "adb is not recognized"
הוסף ל-PATH:
```powershell
setx PATH "%PATH%;%LOCALAPPDATA%\Android\Sdk\platform-tools"
# סגור ופתח מחדש את PowerShell
```

### האמולטור איטי מאוד
- סגור אפליקציות אחרות במחשב
- תן לאמולטור יותר RAM ב-AVD Settings
- בחר מכשיר קטן יותר (Pixel 5 במקום Pixel 7)

### "Android SDK not found"
```powershell
setx ANDROID_HOME "%LOCALAPPDATA%\Android\Sdk"
# סגור ופתח מחדש את PowerShell
```

---

## 📊 גדלי הורדה משוערים

- Android Studio: ~1.2 GB
- Android SDK: ~2-3 GB
- System Image: ~500 MB
- **סה"כ: ~4-5 GB**

זמן התקנה: 20-30 דקות (תלוי במהירות אינטרנט)

---

## ❓ שאלות?

פתח Issue ב-GitHub: https://github.com/dorongut1/HereForYou/issues

---

**בהצלחה! 🚀**
