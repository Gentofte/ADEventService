@echo off

SET DOTNETFX4=%SystemRoot%\Microsoft.NET\Framework\v4.0.30319
SET PATH=%PATH%;%DOTNETFX4%

echo Installing service...
echo --------------------------------------------------
InstallUtil /i /ShowCallStack ADEventService.exe
echo --------------------------------------------------
echo Done.
