# 🐛 פתרון בעיות - Windows

## 🚨 ה-EXE לא עובד? הנה מה לעשות:

### שלב 1: בדוק שה-EXE קיים

```powershell
cd C:\Disk1\HereForYou
ls bin\Release\net10.0-windows10.0.19041.0\win-x64\publish\HereForYou.exe
```

אם הקובץ קיים - עבור לשלב 2
אם אין - הרץ: `dotnet publish -f net10.0-windows10.0.19041.0 -c Release`

---

### שלב 2: נסה להריץ עם test-run.bat

```powershell
.\test-run.bat
```

זה ייתן לך מידע על שגיאות אם יש.

---

### שלב 3: נסה להריץ מקוד מקור

במקום ה-EXE, נסה להריץ ישירות:

```powershell
dotnet run -f net10.0-windows10.0.19041.0
```

אם זה **לא עובד** - תראה שגיאה במסך
אם זה **עובד** - הבעיה בבניית ה-EXE

---

### שלב 4: בדוק Windows Event Viewer

אם האפליקציה פשוט נסגרת בלי הודעה:

1. לחץ **Win + R**
2. הקלד: `eventvwr.msc`
3. לחץ **OK**
4. עבור ל: **Windows Logs** → **Application**
5. חפש שגיאות אדומות מ ".NET Runtime" או "HereForYou"

---

## 🔍 בעיות נפוצות ופתרונות

### בעיה 1: "האפליקציה נסגרת מיד"

**סיבה:** חסר .NET 10 Runtime או שגיאה באתחול

**פתרון:**
```powershell
# בדוק גרסת .NET
dotnet --version
# צריך לראות: 10.0.100 או גבוה יותר

# אם לא מותקן - התקן:
winget install Microsoft.DotNet.SDK.10
```

---

### בעיה 2: "המסך מהבהב ונסגר"

**סיבה:** שגיאה בקוד או ב-XAML

**פתרון:** הרץ מקוד מקור כדי לראות השגיאה
```powershell
dotnet run -f net10.0-windows10.0.19041.0
```

**אם רואה שגיאה - העתק אותה ו:**
1. פתח Issue ב-GitHub
2. או רשום לי כאן מה השגיאה

---

### בעיה 3: "System.IO.FileNotFoundException: Could not load file 'hereforyou.dll'"

**סיבה:** Build לא הושלם או קבצים חסרים

**פתרון:**
```powershell
# נקה ובנה מחדש
dotnet clean
dotnet build -f net10.0-windows10.0.19041.0 -c Release
dotnet publish -f net10.0-windows10.0.19041.0 -c Release
```

---

### בעיה 4: "The application requires a higher version of .NET"

**סיבה:** .NET 10 לא מותקן או גרסה ישנה

**פתרון:**
```powershell
# הסר גרסאות ישנות והתקן 10
winget install Microsoft.DotNet.SDK.10 --force
```

---

### בעיה 5: "Access Denied" או הרשאות

**סיבה:** Windows Defender או antivirus חוסם

**פתרון:**
1. לחץ ימין על `HereForYou.exe`
2. בחר "Properties"
3. טאב "General" → "Unblock" (אם יש)
4. לחץ "OK"

או הרץ כמנהל:
```powershell
# לחץ ימין על PowerShell → "Run as Administrator"
cd C:\Disk1\HereForYou
dotnet run -f net10.0-windows10.0.19041.0
```

---

### בעיה 6: "Microsoft.UI.Xaml.dll not found"

**סיבה:** חסרים קבצי WinUI 3

**פתרון:**
```powershell
# התקן MAUI workload מחדש
dotnet workload install maui --force
```

---

## 🎯 המלצה שלי

**אם ה-EXE לא עובד:**

1. **נסה להריץ מקוד מקור:**
   ```powershell
   dotnet run -f net10.0-windows10.0.19041.0
   ```

2. **אם זה עובד** - אז הקוד תקין, רק ה-publish הproblem
   - בנה מחדש: `dotnet publish -f net10.0-windows10.0.19041.0 -c Release`

3. **אם גם זה לא עובד** - תראה שגיאה ספציפית
   - העתק את השגיאה
   - פתח Issue ב-GitHub עם הפרטים

---

## 🔬 Debug מתקדם

אם שום דבר לא עוזר, הרץ עם debug logging:

```powershell
# הגדר משתני סביבה לdebug
$env:DOTNET_EnableEventPipe="1"
$env:COMPlus_LogLevel="4"

# הרץ
dotnet run -f net10.0-windows10.0.19041.0

# זה ידפיס המון לוגים - העתק אותם
```

---

## 📊 מידע שכדאי לאסוף כשמדווחים על בעיה

1. **גרסת Windows:**
   ```powershell
   winver
   ```

2. **גרסת .NET:**
   ```powershell
   dotnet --version
   dotnet --list-sdks
   dotnet --list-runtimes
   ```

3. **Build output:**
   ```powershell
   dotnet build -f net10.0-windows10.0.19041.0 -c Debug > build.log 2>&1
   # העתק את build.log
   ```

4. **השגיאה המדויקת:**
   - צילום מסך
   - העתקה מ-Event Viewer
   - Output של `dotnet run`

---

## 💡 טיפ אחרון

**אם הכל נכשל - נסה Visual Studio:**

1. פתח Visual Studio 2022
2. File → Open → Project/Solution
3. בחר `C:\Disk1\HereForYou\HereForYou.sln`
4. לחץ **F5**

Visual Studio יראה לך בדיוק מה הבעיה ואיפה.

---

**צריך עזרה? פתח Issue:**
https://github.com/dorongut1/HereForYou/issues
