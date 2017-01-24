@echo off

SET DOTNETFX4=%SystemRoot%\Microsoft.NET\Framework\v4.0.30319
SET PATH=%PATH%;%DOTNETFX4%

echo UNinstalling service...
echo --------------------------------------------------
InstallUtil /u /ShowCallStack ADEventService.exe
echo --------------------------------------------------
echo Done.
