@echo off

cls

:: Update NuGet executable.
tools\nuget\nuget.exe u -self

:: Build
cls

:: Install FAKE.
"tools\nuget\nuget.exe" install "FAKE" -source "https://nuget.org/api/v2/" -OutputDirectory "tools" -ExcludeVersion

cls

:: Run the FAKE build script.
"tools\FAKE\tools\Fake.exe" build.fsx %*

:: Quit
exit /b %errorlevel%