@echo off
echo ===================================
echo HereForYou Debug Launcher
echo ===================================
echo.

echo Checking .NET installation...
dotnet --version
if %ERRORLEVEL% NEQ 0 (
    echo ERROR: .NET is not installed or not in PATH
    echo Please install .NET 10 SDK from: https://dotnet.microsoft.com/download
    pause
    exit /b 1
)
echo.

echo Checking MAUI workload...
dotnet workload list
echo.

echo Building application...
dotnet build -f net10.0-windows10.0.19041.0 -c Debug
if %ERRORLEVEL% NEQ 0 (
    echo ERROR: Build failed!
    pause
    exit /b 1
)
echo.

echo Running application...
echo If the app doesn't appear, check for errors below:
echo.
dotnet run -f net10.0-windows10.0.19041.0 -c Debug --no-build

pause
