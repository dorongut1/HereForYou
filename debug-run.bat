@echo off
echo ===================================
echo Debug Run - HereForYou
echo ===================================
echo.

cd /d "%~dp0"

echo Setting debug environment variables...
set DOTNET_DebugWriteToStdErr=1
set COREHOST_TRACE=1
set COREHOST_TRACEFILE=debug.log

echo.
echo Running with full debug output...
echo This will show the REAL error!
echo.

dotnet run -f net10.0-windows10.0.19041.0 --verbosity detailed 2>&1

echo.
echo ===================================
echo Check the output above for the actual error
echo ===================================
echo.
echo Also check: debug.log file in this folder
echo.

pause
