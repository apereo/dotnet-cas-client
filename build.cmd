@echo off

cls

:: Install NuGet
echo Downloading NuGet...
PowerShell.exe -Command "New-Item -ItemType Directory -Force -Path 'tools\nuget' | out-null"
PowerShell.exe -Command "Invoke-WebRequest https://dist.nuget.org/win-x86-commandline/latest/nuget.exe -OutFile tools\nuget\nuget.exe"
echo Downloaded NuGet!

:: Install FAKE
echo Downloading FAKE...
"tools\nuget\nuget.exe" install "FAKE" -Version "4.62.1" -source "https://nuget.org/api/v2/" -OutputDirectory "tools" -ExcludeVersion
echo Downloaded FAKE!

:: Run the FAKE build script.
"tools\FAKE\tools\Fake.exe" build.fsx %*

:: Quit
exit /b %errorlevel%