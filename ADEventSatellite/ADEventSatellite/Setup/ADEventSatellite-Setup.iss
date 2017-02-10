; Script generated by the Inno Script Studio Wizard.
; SEE THE DOCUMENTATION FOR DETAILS ON CREATING INNO SETUP SCRIPT FILES!

;#define TargetDir "C:\NTVOL1\GIT2\ADEventService\ADEventSatellite\ADEventSatellite\ADEventSatellite\bin\Release"
;#define TargetPath "C:\NTVOL1\GIT2\ADEventService\ADEventSatellite\ADEventSatellite\ADEventSatellite\bin\Release\ADEventSatellite.exe"
;#define SolutionDir "C:\NTVOL1\GIT2\ADEventService\ADEventSatellite\ADEventSatellite"

#define MyAppName GetStringFileInfo("C:\NTVOL1\GIT2\ADEventService\ADEventSatellite\ADEventSatellite\ADEventSatellite\bin\Release\ADEventSatellite.exe", "ProductName")
#define MyAppVersion GetStringFileInfo("C:\NTVOL1\GIT2\ADEventService\ADEventSatellite\ADEventSatellite\ADEventSatellite\bin\Release\ADEventSatellite.exe", "ProductVersion")
#define MyAppFileVersion GetStringFileInfo("C:\NTVOL1\GIT2\ADEventService\ADEventSatellite\ADEventSatellite\ADEventSatellite\bin\Release\ADEventSatellite.exe", "FileVersion")
#define MyAppPublisher GetStringFileInfo("C:\NTVOL1\GIT2\ADEventService\ADEventSatellite\ADEventSatellite\ADEventSatellite\bin\Release\ADEventSatellite.exe", "CompanyName")
#define MyAppCopyright GetStringFileInfo("C:\NTVOL1\GIT2\ADEventService\ADEventSatellite\ADEventSatellite\ADEventSatellite\bin\Release\ADEventSatellite.exe", "LegalCopyright")
#define MyAppURL "https://github.com/gentofte/ADEventService"
#define MyAppExeName "ADEventSatellite.exe"
                                  
[Setup]
; NOTE: The value of AppId uniquely identifies this application.
; Do not use the same AppId value in installers for other applications.
; (To generate a new GUID, click Tools | Generate GUID inside the IDE.)
AppId={{7EECF5AE-2318-4563-BB7B-BCB53519F8FF}
AppName={#MyAppName}
AppVersion={#MyAppVersion}
AppVerName={#MyAppName} {#MyAppVersion}
AppPublisher={#MyAppPublisher}
AppPublisherURL={#MyAppURL}
AppSupportURL={#MyAppURL}
AppUpdatesURL={#MyAppURL}
AppCopyright={#MyAppCopyright}
DefaultDirName=C:\NTVOL1\bin\{#MyAppName}
DisableProgramGroupPage=yes
OutputDir={#SolutionDir}\Setup
OutputBaseFilename=ADEventSatellite-Setup
Compression=lzma
SolidCompression=yes

[Languages]
Name: "english"; MessagesFile: "compiler:Default.isl"

[Files]
Source: {#TargetDir}\{#MyAppExeName}; DestDir: "{app}"; Flags: ignoreversion
Source: {#TargetDir}\{#MyAppExeName}.config; DestDir: "{app}"; Flags: ignoreversion

Source: {#TargetDir}\*.dll; DestDir: "{app}"; Flags: ignoreversion

Source: {#TargetDir}\LICENSE; DestDir: "{app}"; Flags: ignoreversion
Source: {#TargetDir}\README.md; DestDir: "{app}"; Flags: ignoreversion

[Run]
Filename: "{app}\{#MyAppExeName}"; Parameters: "--install"

[UninstallRun]
Filename: "{app}\{#MyAppExeName}"; Parameters: "--uninstall"
