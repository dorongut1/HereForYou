@echo off
echo ===================================
echo HereForYou - Production Launcher
echo ===================================
echo.

cd /d "%~dp0"

set EXE_PATH=bin\Release\net10.0-windows10.0.19041.0\win-x64\publish\HereForYou.exe

if exist "%EXE_PATH%" (
    echo Starting HereForYou from publish folder...
    echo.
    start "" "%EXE_PATH%"
    echo.
    echo If the app doesn't open, try running from Visual Studio
    echo or check Windows Event Viewer for errors.
) else (
    echo ERROR: Published executable not found!
    echo Expected location: %EXE_PATH%
    echo.
    echo Building and publishing application...
    dotnet publish -f net10.0-windows10.0.19041.0 -c Release
    echo.
    if exist "%EXE_PATH%" (
        echo Build successful! Starting application...
        start "" "%EXE_PATH%"
    ) else (
        echo Build failed or executable not created.
        echo Please check build errors above.
    )
)

pause
