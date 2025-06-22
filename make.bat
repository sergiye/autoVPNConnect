@echo off

rem FOR /F "tokens=* USEBACKQ" %%F IN (`"%ProgramFiles(x86)%\Microsoft Visual Studio\Installer\vswhere.exe" -latest -prerelease -products * -requires Microsoft.Component.MSBuild -find MSBuild\**\Bin\MSBuild.exe`) DO (
rem SET msbuild="%%F"
rem )
rem ECHO %msbuild%

rem @%msbuild% AutoVPNConnect.sln /t:restore /p:RestorePackagesConfig=true
rem @%msbuild% AutoVPNConnect.sln /t:Rebuild /p:DebugType=None /p:Configuration=Release

dotnet build AutoVPNConnect.sln -c Release -p:DebugType=None -p:Platform="Any CPU"

if errorlevel 1 goto error

goto exit
:error
pause
:exit
