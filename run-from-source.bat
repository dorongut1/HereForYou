@echo off
echo ===================================
echo Running HereForYou from Source Code
echo ===================================
echo.

cd /d "%~dp0"

echo This should work even if the EXE doesn't!
echo.
echo Building and running...
echo.

dotnet run -f net10.0-windows10.0.19041.0

if %ERRORLEVEL% NEQ 0 (
    echo.
    echo ===================================
    echo ERROR: Failed to run from source
    echo ===================================
    echo.
    echo Please copy the error message above and let me know.
    echo.
)

pause
