@echo off
echo ===================================
echo Testing HereForYou Application
echo ===================================
echo.

cd /d "%~dp0"

set EXE_PATH=bin\Release\net10.0-windows10.0.19041.0\win-x64\publish\HereForYou.exe

if not exist "%EXE_PATH%" (
    echo ERROR: EXE not found at: %EXE_PATH%
    echo.
    echo Building the application first...
    dotnet publish -f net10.0-windows10.0.19041.0 -c Release
    echo.
)

echo Running: %EXE_PATH%
echo.
echo If the app doesn't appear within 5 seconds:
echo 1. Check Windows Event Viewer ^(eventvwr.msc^)
echo 2. Look under: Application Logs
echo 3. Look for errors from "HereForYou" or ".NET Runtime"
echo.

"%EXE_PATH%"

if %ERRORLEVEL% NEQ 0 (
    echo.
    echo ===================================
    echo ERROR: Application exited with code: %ERRORLEVEL%
    echo ===================================
    echo.
    echo Common issues:
    echo 1. Missing .NET 10 Runtime
    echo 2. Missing dependencies
    echo 3. Corrupt build
    echo.
    echo Checking .NET installation...
    dotnet --version
    echo.
    echo Try running from source instead:
    echo   dotnet run -f net10.0-windows10.0.19041.0
    echo.
)

pause
